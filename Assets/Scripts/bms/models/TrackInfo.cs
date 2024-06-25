using System;
using System.Collections.Generic;

namespace BMS
{
    public class TrackInfo : IComparable
    {
        public string genre = "";
        public string title = "";
        public string subTitle = "";
        public string artist = "";
        public string subArtist = "";
        public string path = "";
        public string stageFile = "";
        public double bpm = 0;
        public double total = 0;
        public int playerType = 0;
        public int playLevel = 0;
        public int rank = 0;
        public Dictionary<int, string> audioFileNames = new Dictionary<int, string>();
        public Dictionary<int, string> imageFileNames = new Dictionary<int, string>();
        public Dictionary<int, string> videoFileNames = new Dictionary<int, string>();
        public Dictionary<int, double> bpmTable = new Dictionary<int, double>();
        public Dictionary<int, double> stopTable = new Dictionary<int, double>();
        public int lnobj = 0;
        public int lnType = 1;
        public int[] selectedRandom;

        public int CompareTo(object trackInfo)
        {
            if (playLevel > (trackInfo as TrackInfo).playLevel)
                return 1;
            else if (playLevel == (trackInfo as TrackInfo).playLevel)
                return 0;
            else
                return -1;
        }
    }
}
