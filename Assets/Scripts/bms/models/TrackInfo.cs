using System;
using System.Collections.Generic;

public class TrackInfo : IComparable
{
    public string genre = "";
    public string title = "";
    public string artist = "";
    public string path = "";
    public string stageFile = "";
    public float bpm = 0;
    public float total = 0;
    public int playerType = 0;
    public int playLevel = 0;
    public int rank = 0;
    public Dictionary<int, string> audioFileNames = new Dictionary<int, string>();
    public Dictionary<string, string> imageFileNames = new Dictionary<string, string>();
    public Dictionary<string, float> bpmTable = new Dictionary<string, float>();
    public Dictionary<string, int> stopTable = new Dictionary<string, int>();
    public List<string> lnobjList = new List<string>();
    public bool isLnobj = false;
    public int lnType = 1;
    public int barCount = 0;

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
