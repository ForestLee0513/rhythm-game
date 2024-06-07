using System.Collections.Generic;

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
        public int bar = default;
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

        public void SortObjects()
        {
            bpmList.Sort();
            bgmKeySoundChannel.Sort();
            stopList.Sort();
            bgaSequenceFrameList.Sort();
            foreach (Line line in lines)
            {
                line.NoteList.Sort();
                line.LandMineList.Sort();
            }
        }
    }
}