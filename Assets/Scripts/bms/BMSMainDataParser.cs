using System;
using System.IO;
using UnityEngine;

namespace BMS
{
    public class BMSMainDataParser : ChartDecoder
    {
        private Pattern pattern = new Pattern();
        public Pattern Pattern { get { return pattern; }}
        private int LNBits = 0;

        public BMSMainDataParser(string path) : base(path)
        {
            parseData += ParseMainData;
            ReadFile();
            // 패턴 파싱을 다하고나서 노트 bar와 beat를 기준으로 정렬
            pattern.CalculateBeatTimings(TrackInfo.bpm, TrackInfo.stopTable);
        }

        public BMSMainDataParser(TrackInfo trackInfo): base(trackInfo)
        {
            parseData += ParseMainData;
            ReadFile();

            // 패턴 파싱을 다하고나서 노트 bar와 beat를 기준으로 정렬
            pattern.CalculateBeatTimings(TrackInfo.bpm, TrackInfo.stopTable);
        }

        private void ParseMainData(string line)
        {
            // 메인 데이터 파싱
            string mainDataKey = line.IndexOf(":") > -1 && line.StartsWith("#") ? line.Substring(1, line.IndexOf(":") - 1) : "";
            string mainDataValue = line.IndexOf(":") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(":") + 1) : "";
            if (mainDataKey != "" && mainDataValue != "")
            {
                // 마디
                Int32.TryParse(mainDataKey.Substring(0, 3), out int currentBar);

                if (pattern.totalBarCount < currentBar)
                {
                    pattern.totalBarCount = currentBar;
                }

                string channel = mainDataKey.Substring(3);
                int lane = (channel[1] - '0') - 1;
                // 2P (DP)인 경우 9를 더해서 2P 라인까지 index가 갈 수 있도록 수정
                // Pattern.cs 참고
                if (channel[0] == '2' || channel[0] == '6')
                {
                    lane += 9;
                }

                // 채널 02를 제외하고 모두 36진수로 이루어져 있어 채널 여부로 구분
                if (channel != "02")
                {
                    int beatLength = mainDataValue.Length / 2;

                    for (int i = 0; i < mainDataValue.Length - 1; i += 2)
                    {
                        int beat = i / 2;
                        // Value - 36진수에서 10진수로 파싱된 값
                        int parsedToIntValue = Decode36(mainDataValue.Substring(i, 2));
                        // Debug.Log($"{bar} 마디의 {beat}비트");

                        // 키음이 00이 아닐때만 노트, BGA 등 에셋 배치
                        if (parsedToIntValue == 0)
                        {
                            continue;
                        }

                        // 노트 처리 //
                        if (channel[0] == '1' || channel[0] == '2')
                        {
                            // 롱노트 - LNOBJ 선언 됐을 경우의 처리
                            if (TrackInfo.lnobj == parsedToIntValue)
                            {
                                pattern.AddNote(lane, currentBar, beat, beatLength, parsedToIntValue, Note.NoteFlagState.LnEnd);
                                continue;
                            }
                            else
                            {
                                // 일반 노트
                                pattern.AddNote(lane, currentBar, beat, beatLength, parsedToIntValue, Note.NoteFlagState.Default);
                                continue;
                            }
                        }

                        if (channel[0] == '5' || channel[0] == '6')
                        {
                            if ((LNBits & (1 << lane)) != 0)
                            {
                                pattern.AddNote(lane, currentBar, beat, beatLength, -1, Note.NoteFlagState.LnEnd);
                                LNBits &= ~(1 << lane); //erase bit
                                continue;
                            }
                            else
                            {
                                LNBits |= (1 << lane); //write bit
                                pattern.AddNote(lane, currentBar, beat, beatLength, parsedToIntValue, Note.NoteFlagState.LnStart);
                                continue;
                            }
                        }

                        // BGM CHANNEL //
                        if (channel == "01")
                        {
                            pattern.AddBGMKeySound(currentBar, beat, beatLength, parsedToIntValue);
                            continue;
                        }

                        // BPM CHANNEL //
                        if (channel == "03")
                        {
                            double bpm = int.Parse(mainDataValue.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                            pattern.AddBPMTable(currentBar, beat, beatLength, bpm);
                            continue;
                        }

                        // BGA SEQUENCE //
                        if (channel == "04")
                        {
                            if (TrackInfo.imageFileNames.ContainsKey(parsedToIntValue))
                            {
                                pattern.AddBGASequenceFrames(currentBar, beat, beatLength, parsedToIntValue, BGASequence.BGAFlagState.Image);
                            }
                            else if (TrackInfo.videoFileNames.ContainsKey(parsedToIntValue))
                            {
                                pattern.AddBGASequenceFrames(currentBar, beat, beatLength, parsedToIntValue, BGASequence.BGAFlagState.Video);
                            }

                            continue;
                        }

                        // BPM CHANNEL 이전 BPM추가 //
                        if (channel == "08")
                        {
                            pattern.AddBPMTable(currentBar, beat, beatLength, TrackInfo.bpmTable[parsedToIntValue]);
                            continue;
                        }

                        // 변속 //
                        if (channel == "09")
                        {
                            pattern.AddStop(currentBar, beat, beatLength, parsedToIntValue);
                            continue;
                        }
                    }
                }
                else
                {
                    // 변박 //
                    // 마디 내 박자 수 (1이 4/4 박자임.)
                    Double.TryParse(mainDataValue, out double beatMeasureLength);
                    pattern.AddBeatMeasureLength(currentBar, beatMeasureLength);
                }
            }
        }
    }
}
