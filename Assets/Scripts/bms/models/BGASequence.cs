namespace BMS
{
    public class BGASequence : BMSObject
    {
        public enum BGAFlagState
        {
            Image,
            Video
        }

        public int BgaSequenceFrame { get; private set; }
        public BGAFlagState Flag { get; private set; }

        public BGASequence(int bar, double beat, double beatLength, int bgaSequenceFrame, BGAFlagState flag) : base(bar, beat, beatLength)
        {
            BgaSequenceFrame = bgaSequenceFrame;
            Flag = flag;
        }

        public void SetFlag(BGAFlagState flag)
        {
            Flag = flag;
        }
    }
}