namespace BMS
{
    public class Stop : BMSObject
    {
        public int Key { get; private set; }
        public Stop(int bar, double beat, double beatLength, int stop) : base(bar, beat, beatLength)
        {
            Key = stop;
        }
    }
}
