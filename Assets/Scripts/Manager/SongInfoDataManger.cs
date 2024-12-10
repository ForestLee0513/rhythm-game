public class SongInfoDataManager : SQLiteManager
{
    public SongInfoDataManager(string dbFilePath) : base(dbFilePath)
    {
        Init();
        CreateTableIfNotExists(SongInfo.Table);
        CreateTableIfNotExists(FolderInfo.Table);
    }
}
