namespace BMS
{
    public class Note : BMSObject
    {
        public enum NoteFlagState
        {
            Default,
            LnStart,
            LnEnd,
            Mine,
            BGM
        }

        public int KeySound { get; private set; }
        public NoteFlagState Flag { get; private set; }

        public Note(int bar, double beat, double beatLength, int keySound, NoteFlagState flag) : base(bar, beat, beatLength)
        {
            KeySound = keySound;
            Flag = flag;
        }

        public void SetFlag(NoteFlagState flag)
        {
            Flag = flag;
        }
    }
    }