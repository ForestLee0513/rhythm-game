using System.Collections.Generic;

public class SongInfoDataManager : SQLiteManager
{
    public SongInfoDataManager(string dbFilePath) : base(dbFilePath)
    {
        Init();
        CreateTableIfNotExists(SongInfo.Table);
        CreateTableIfNotExists(FolderInfo.Table);
    }

    public List<T> SearchFolderPath<T>(string folderPath) where T : new()
    {
        return FindValues<T>(FolderInfo.Table.Name, "PATH", folderPath);
    }
}
