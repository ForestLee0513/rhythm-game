public class Note : BMSObject
{
    public enum NoteFlagState
    {
        Default,
        LnStart,
        LnEnd,
        Mine
    }

    public int KeySound { get; private set; }
    public NoteFlagState Flag { get; private set; }

    public Note(int bar, float beat, float beatLength, int keySound, NoteFlagState flag) : base(bar, beat, beatLength)
    {
        KeySound = keySound;
        Flag = flag;
    }

    public void SetFlag(NoteFlagState flag)
    {
        Flag = flag;
    }
}