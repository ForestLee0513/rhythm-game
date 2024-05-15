public class BPM : BMSObject
{
    public double Bpm { get; private set; }
    public BPM(int bar, double beat, double beatLength, double bpm) : base(bar, beat, beatLength)
    {
        Bpm = bpm;
    }
}