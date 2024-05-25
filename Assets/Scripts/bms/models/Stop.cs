namespace BMS
{
    public class Stop : BMSObject
    {
        public double Key { get; private set; }
        public Stop(int bar, double beat, double beatLength, double stop) : base(bar, beat, beatLength)
        {
            Key = stop;
        }
    }
}