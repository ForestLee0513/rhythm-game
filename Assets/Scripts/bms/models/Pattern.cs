using System.Collections.Generic;

public class Pattern
{
    public int bar = default;
    public Dictionary<int, float> beatMeasureLengthTable = new Dictionary<int, float>();
    public Line[] lines = new Line[18]; // DP랑 2P대응을 위해 9 x 9 형식으로 대응.

    public Pattern()
    {
        for (int i = 0; i < lines.Length; ++i)
        {
            lines[i] = new Line();
        }
    }

    // 변박
    public void AddBeatMeasureLength(int bar, float beatMeasureLength)
    {
        beatMeasureLengthTable.Add(bar, beatMeasureLength);
    }

    // 일반 노트
    public void AddNote(int line, int bar, float beat, float beatLength, int keySound, Note.NoteFlagState flag)
    {
        if (flag == Note.NoteFlagState.LnEnd)
        {
            lines[line].NoteList[lines[line].NoteList.Count - 1].SetFlag(Note.NoteFlagState.LnStart);
        }
        
        lines[line].NoteList.Add(new Note(bar, beat, beatLength, keySound, flag));
    }
}