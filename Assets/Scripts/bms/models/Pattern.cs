using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
            if (flag == Note.NoteFlagState.LnEnd)
            {
                lines[line].NoteList[lines[line].NoteList.Count - 1].SetFlag(Note.NoteFlagState.LnStart);
            }
            
            lines[line].NoteList.Add(new Note(bar, beat, beatLength, keySound, flag));
        }

        private double GetBeatMeasureLength(int bar) => beatMeasureLengthTable.ContainsKey(bar) ? beatMeasureLengthTable[bar] : 1.0;

        private double GetBPM(double beat)
        {
            if (bpmList.Count == 1) return bpmList[0].Bpm;
            int idx = bpmList.Count - 1;
            while (idx > 0 && beat >= bpmList[--idx].Beat) ;
            return bpmList[idx + 1].Bpm;
        }

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
            for (i = bpmList.Count - 1; i > 0 && obj.Beat > bpmList[i - 1].Beat; --i)
            {
                timing += (bpmList[i - 1].Beat - bpmList[i].Beat) / bpmList[i].Bpm * 60;
            }
            timing += (obj.Beat - bpmList[i].Beat) / bpmList[i].Bpm * 60;
            return timing;
        }
        
        public void CalculateBeatTimings(double defaultBPM, Dictionary<int, double> stopTable)
        {
            if (bpmList.Count == 0 || (bpmList.Count > 0 && bpmList[bpmList.Count - 1].Beat != 0))
            {
                AddBPMTable(0, 0, 1, defaultBPM);
            }
            bpmList[bpmList.Count - 1].Timing = 0;
            for (int i = bpmList.Count - 2; i > -1; --i)
            {
                bpmList[i].Timing = bpmList[i + 1].Timing + (bpmList[i].Beat - bpmList[i + 1].Beat) / (bpmList[i + 1].Bpm / 60);
            }

            foreach (Stop s in stopList)
            {
                s.CalculateBeat(GetPreviousBarBeatSum(s.Bar), GetBeatMeasureLength(s.Bar));
                s.Timing = GetTimingInSecond(s);
            }

            foreach (BGASequence bgaSequence in bgaSequenceFrameList)
            {
                bgaSequence.CalculateBeat(GetPreviousBarBeatSum(bgaSequence.Bar), GetBeatMeasureLength(bgaSequence.Bar));
                bgaSequence.Timing = GetTimingInSecond(bgaSequence);
                int idx = stopList.Count - 1;
                double sum = 0;
                while (idx > 0 && bgaSequence.Beat > stopList[--idx].Beat) sum += stopTable[stopList[idx].Bar] / GetBPM(stopList[idx].Beat) * 240;
                bgaSequence.Timing += sum;
            }

            foreach (Note bgm in bgmKeySoundChannel)
            {
                bgm.CalculateBeat(GetPreviousBarBeatSum(bgm.Bar), GetBeatMeasureLength(bgm.Bar));
                bgm.Timing = GetTimingInSecond(bgm);
            }

            foreach (Line line in lines)
            {
                foreach (Note note in line.NoteList)
                {
                    note.CalculateBeat(GetPreviousBarBeatSum(note.Bar), GetBeatMeasureLength(note.Bar));
                    note.Timing = GetTimingInSecond(note);
                }
                foreach (Note mineList in line.LandMineList)
                {
                    mineList.CalculateBeat(GetPreviousBarBeatSum(mineList.Bar), GetBeatMeasureLength(mineList.Bar));
                    mineList.Timing = GetTimingInSecond(mineList);
                }
            }
        }
    }
}