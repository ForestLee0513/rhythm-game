using Mono.Data.Sqlite;

public class SongInfoDataManager : SQLiteManager
{
    public SongInfoDataManager(string dbFilePath) : base(dbFilePath)
    {
        Init(SongInfo.Table);
    }
}
