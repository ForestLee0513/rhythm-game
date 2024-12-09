using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;
using static SQLiteData;

public abstract class SQLiteManager
{
    protected string connectionPath;
    protected string filePath;

    protected SqliteConnection connection;
    protected SqliteCommand command;
    protected SqliteDataReader reader;

    protected SQLiteManager(string fileName)
    {
        filePath = $"{Application.persistentDataPath}/{fileName}.db";
        connectionPath = $"URI=file:{Application.persistentDataPath}/{fileName}.db";
    }

    public void Init(Table table)
    {
        CreateFileIfNotExists();
        OpenDatabase();
        CreateTableIfNotExists(table);
    }

    public void CreateFileIfNotExists()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                using FileStream fs = File.Create(filePath);
                fs.Close();
            }
        }
        catch 
        {
            Debug.Log("Failed to create DB file.");
        }
    }

    public void CreateTableIfNotExists(Table table)
    {
        Debug.Log($"Table name is {table.Name}");
    }

    public void OpenDatabase()
    {
        connection = new SqliteConnection(connectionPath);
        connection.Open();
        Debug.Log("Connected to database");
    }

    public void CloseDatabase()
    {
        if (command != null)
        {
            command.Dispose();
        }
        command = null;

        if (reader != null)
        {
            reader.Dispose();
        }
        reader = null;

        if (connection != null)
        {
            connection.Close();
        }
        connection = null;
        
        Debug.Log("Disconnected from database.");
    }
}
