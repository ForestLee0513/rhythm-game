using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace BMS
{
    public enum PatternLaneType
    {
        Default,
        bpmList,
        bgmKeySoundChannel,
        stopList,
        bgaSequenceFrameList,
    }

    public class Pattern
    {
        public int totalBarCount = default;
        public Dictionary<int, double> beatMeasureLengthTable = new Dictionary<int, double>();
        public List<BPM> bpmList = new List<BPM>();
        public List<Note> bgmKeySoundChannel = new List<Note>();
        public List<Stop> stopList = new List<Stop>();
        public List<BGASequence> bgaSequenceFrameList = new List<BGASequence>();
        public Line[] lines = new Line[18]; // DP(1P + 2P)대응을 위해 9 + 9 형식으로 대응. (17번은 페달이지만 미대응.)

        public Pattern()
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i] = new Line();
            }
        }

        // 변박
        public void AddBeatMeasureLength(int bar, double beatMeasureLength)
        {
            beatMeasureLengthTable.Add(bar, beatMeasureLength);
        }
        
        // BPM 테이블 추가
        public void AddBPMTable(int bar, double beat, double beatLength, double bpmKey)
        {
            bpmList.Add(new BPM(bar, beat, beatLength, bpmKey));
        }

        private void AddFirstBPMTable(int bar, double beat, double beatLength, double bpmKey)
        {
            bpmList.Insert(0, new BPM(bar, beat, beatLength, bpmKey));
        }
        
        // 마디 별 BGM 사운드 추가
        public void AddBGMKeySound(int bar, double beat, double beatLength, int keySound)
        {
            bgmKeySoundChannel.Add(new Note(bar, beat, beatLength, keySound, Note.NoteFlagState.BGM));
        }

        // 정지기믹 리스트 추가
        public void AddStop(int bar, double beat,double beatLength, int stopKey)
        {
            stopList.Add(new Stop(bar, beat, beatLength, stopKey));
        }

        // BGA 시퀀스 프레임 추가
        public void AddBGASequenceFrames(int bar, double beat,double beatLength, int bgaSequenceFrame, BGASequence.BGAFlagState flag)
        {
            bgaSequenceFrameList.Add(new BGASequence(bar, beat, beatLength, bgaSequenceFrame, flag));
        }

        // 일반 노트
        public void AddNote(int line, int bar, double beat, double beatLength, int keySound, Note.NoteFlagState flag)
        {
            //if (lines[line].NoteList.Count >= 2 && lines[line].NoteList[lines[line].NoteList.Count - 2].Flag == Note.NoteFlagState.LnStart)
            //{
            //    lines[line].NoteList[lines[line].NoteList.Count - 1].SetFlag(Note.NoteFlagState.LnEnd);
            //}
            
            lines[line].NoteList.Add(new Note(bar, beat, beatLength, keySound, flag));
        }

        private double GetBeatMeasureLength(int bar) => beatMeasureLengthTable.ContainsKey(bar) ? beatMeasureLengthTable[bar] : 1.0;

        private double GetPreviousBarBeatSum(int bar)
        {
            double sum = 0;
            for (int i = 0; i < bar; ++i)
            {
                sum += 4.0 * GetBeatMeasureLength(i);
            }
            return sum;
        }

        private double GetTimingInSecond(BMSObject obj)
        {
            double timing = 0;
            int i;
            for (i = 0; i < bpmList.Count - 1 && obj.Beat > bpmList[i + 1].Beat; ++i)
            {
                timing += (bpmList[i + 1].Beat - bpmList[i].Beat) / bpmList[i].Bpm * 60;
            }
            timing += (obj.Beat - bpmList[i].Beat) / bpmList[i].Bpm * 60;
            return timing;
        }

        private double GetBPM(double beat)
        {
            if (bpmList.Count == 1) return bpmList[0].Bpm;
            int idx = 0;
            while (idx < bpmList.Count - 1 && beat >= bpmList[idx + 1].Beat)
            {
                idx++;
            }
            return bpmList[idx].Bpm;
        }

        private double CalculateStopTiming(BMSObject bmsObj, Dictionary<int, double> stopTable)
        {
            double sum = 0;
            int idx = 0;
            if (stopTable.Count > 1)
            {
                while (idx < stopList.Count - 1 && bmsObj.Beat > stopList[idx].Beat)
                {
                    sum += stopTable[stopList[idx].Key] / GetBPM(stopList[idx].Beat) * 240;
                    idx++;
                }
            }
            else if (stopTable.Count == 1 && bmsObj.Beat > stopList[0].Beat)
            {
                sum += stopTable[stopList[0].Key] / GetBPM(stopList[0].Beat) * 240;
            }
            
            return sum;
        }

        public void CalculateBeatTimings(double defaultBPM, Dictionary<int, double> stopTable)
        {
            foreach (BPM bpm in bpmList)
            {
                bpm.CalculateBeat(GetPreviousBarBeatSum(bpm.Bar), GetBeatMeasureLength(bpm.Bar));
            }

            if (bpmList.Count == 0 || (bpmList.Count > 0 && bpmList[0].Beat != 0))
            {
                AddBPMTable(0, 0, 1, defaultBPM);
            }
            bpmList[0].Timing = 0;
            bpmList.Sort();

            for (int i = 1; i < bpmList.Count; ++i)
            {
                bpmList[i].Timing = bpmList[i - 1].Timing + (bpmList[i].Beat - bpmList[i - 1].Beat) / (bpmList[i - 1].Bpm / 60);
            }

            foreach (Stop s in stopList)
            {
                s.CalculateBeat(GetPreviousBarBeatSum(s.Bar), GetBeatMeasureLength(s.Bar));
                s.Timing = GetTimingInSecond(s);
            }
            stopList.Sort();

            foreach (BGASequence bgaSequence in bgaSequenceFrameList)
            {
                bgaSequence.CalculateBeat(GetPreviousBarBeatSum(bgaSequence.Bar), GetBeatMeasureLength(bgaSequence.Bar));
                bgaSequence.Timing = GetTimingInSecond(bgaSequence);

            }
            bgaSequenceFrameList.Sort();

            foreach (Note bgm in bgmKeySoundChannel)
            {
                bgm.CalculateBeat(GetPreviousBarBeatSum(bgm.Bar), GetBeatMeasureLength(bgm.Bar));
                bgm.Timing = GetTimingInSecond(bgm);
                double stopTiming = CalculateStopTiming(bgm, stopTable);
                bgm.Timing += stopTiming;
            }
            bgmKeySoundChannel.Sort();

            foreach (Line line in lines)
            {
                foreach (Note note in line.NoteList)
                {
                    note.CalculateBeat(GetPreviousBarBeatSum(note.Bar), GetBeatMeasureLength(note.Bar));
                    note.Timing = GetTimingInSecond(note);
                    double stopTiming = CalculateStopTiming(note, stopTable);
                    note.Timing += stopTiming;
                }

                foreach (Note mineNote in line.LandMineList)
                {
                    mineNote.CalculateBeat(GetPreviousBarBeatSum(mineNote.Bar), GetBeatMeasureLength(mineNote.Bar));
                    mineNote.Timing = GetTimingInSecond(mineNote);
                    double stopTiming = CalculateStopTiming(mineNote, stopTable);
                    mineNote.Timing += stopTiming;
                }

                line.NoteList.Sort();
                line.LandMineList.Sort();
            }
        }
    }
}
