using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public bool PushFolder(string folderPath)
    {
        string folderName = new DirectoryInfo(folderPath).Name;

        try
        {
            Managers.SongInfoDataManager.RunQuery($"INSERT INTO {FolderInfo.Table.Name} VALUES('{folderPath}', '{folderName}')");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }
}
