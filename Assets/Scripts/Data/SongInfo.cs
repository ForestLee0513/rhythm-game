using static SQLiteData;
using static BMSParser.Define;
using static BMSParser.Define.BMSModel;
using static BMSParser.Define.PatternProcessor;

public static class SongInfo
{
    public static Table Table { get; set; } = new Table()
    {
        Name = "SongInfo",
        Fields = new Field[] {
            new("ID", SQLiteDefine.Type.INTEGER, false, true, true, false),

            new("EXTENSION", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("RANKTYPE", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("MD5", SQLiteDefine.Type.TEXT, false, false, false, true),
            new("SHA256", SQLiteDefine.Type.TEXT, false, false, false, true),
            new("SHA512", SQLiteDefine.Type.TEXT, false, false, false, true),
            new("BASE", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("MODE", SQLiteDefine.Type.TEXT, false, false, false, false),

            new("PLAYER", SQLiteDefine.Type.INTEGER, false, false, false, false),
            new("RANK", SQLiteDefine.Type.INTEGER, false, false, false, false),
            new("DEFEXRANK", SQLiteDefine.Type.INTEGER, false, false, false, false),
            new("TOTAL", SQLiteDefine.Type.REAL, false, false, false, false),
            new("VOLWAV", SQLiteDefine.Type.INTEGER, false, false, false, false),
            new("STAGEFILE", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("BANNER", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("BACKBMP", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("PLAYLEVEL", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("DIFFICULTY", SQLiteDefine.Type.INTEGER, false, false, false, false),
            new("TITLE", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("SUBTITLE", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("ARTIST", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("SUBARTIST", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("GENRE", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("BPM", SQLiteDefine.Type.REAL, false, false, false, false),
            new("BPMLIST", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("STOPLIST", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("SCROLLLIST", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("LNOBJ", SQLiteDefine.Type.INTEGER, false, false, false, false),
            new("WAV", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("MIDIFILE", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("BMP", SQLiteDefine.Type.TEXT, false, false, false, false),
            new("LNTYPE", SQLiteDefine.Type.TEXT, false, false, false, false),
        },
    };

    public class Model
    {
        #region Custom Definition
        public BMSModel.Extension EXTENSION { get; set; }
        public RankType RANKTYPE { get; set; } = RankType.RANK;
        public string MD5 { get; set; }
        public string SHA256 { get; set; }
        public string SHA512 { get; set; }
        public Base BASE { get; set; }
        public BMSKey MODE { get; set; }
        #endregion

        #region HEADER
        public int PLAYER { get; set; } = 1;
        public int RANK { get; set; }
        public int DEFEXRANK { get; set; }
        public double TOTAL { get; set; }
        public int VOLWAV { get; set; }
        public string STAGEFILE { get; set; }
        public string BANNER { get; set; }
        public string BACKBMP { get; set; }
        public string PLAYLEVEL { get; set; }
        public int DIFFICULTY { get; set; }
        public string TITLE { get; set; }
        public string SUBTITLE { get; set; }
        public string ARTIST { get; set; }
        public string SUBARTIST { get; set; }
        public string GENRE { get; set; }
        public double BPM { get; set; }
        public double[] BPMLIST { get; set; } = new double[62 * 62];
        public double[] STOPLIST { get; set; } = new double[62 * 62];
        public double[] SCROLLLIST { get; set; } = new double[62 * 62];
        public int LNOBJ { get; set; } = -1;
        public string[] WAV { get; set; } = new string[62 * 62];
        public string MIDIFILE { get; set; }
        public string[] BMP { get; set; } = new string[62 * 62];
        public LNType LNTYPE { get; set; } = LNType.NONE;
        #endregion
    }
}
