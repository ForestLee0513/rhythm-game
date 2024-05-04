using System;
using System.IO;
using System.Text;

public abstract class ChartDecoder
{
    public string path = default;
    private TrackInfo trackInfo = new TrackInfo();
    public TrackInfo TrackInfo { get { return trackInfo; } private set { trackInfo = value; } }
    public delegate void ParseDataDelegate(string line);
    public ParseDataDelegate parseData;

    public ChartDecoder(string path)
    {
        this.path = path;
        trackInfo.path = path;
    }
    
    public ChartDecoder(TrackInfo trackInfo)
    {
        path = trackInfo.path;
    }

    public void ReadFile()
    {
        using (var reader = new StreamReader(path, Encoding.GetEncoding(932)))
        {
            do
            {
                string line = reader.ReadLine();

                parseData(line);
            } while (!reader.EndOfStream);
        }
    }

    // Base36 //
    public static int Decode36(string str)
    {
        if (str.Length != 2) return -1;

        int result = 0;
        if (str[1] >= 'A')
            result += str[1] - 'A' + 10;
        else
            result += str[1] - '0';
        if (str[0] >= 'A')
            result += (str[0] - 'A' + 10) * 36;
        else
            result += (str[0] - '0') * 36;

        return result;
    }
}