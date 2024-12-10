using Mono.Data.Sqlite;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
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

    public void Init()
    {
        CreateFileIfNotExists();
        OpenDatabase();
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
        StringBuilder sqlSb = new();
        bool isPKExists = false;
        int autoIncrementCount = 0;

        // auto increment가 있는지 확인 후 2개 이상이라면 에러 처리
        foreach (Field field in table.Fields)
        {
            if (field.AutoIncrement == true)
            {
                autoIncrementCount++;
            }

            if (autoIncrementCount > 1)
            {
                break;
            }
        }

        if (autoIncrementCount > 1)
        {
            Debug.LogError("AUTO INCREMENT는 1개 이상 적용할 수 없습니다.");
            return;
        }

        // 설정할 PK가 있는지 확인
        foreach (Field field in table.Fields)
        {
            if (field.PrimaryKey == true)
            {
                isPKExists = true;
                break;
            }
        }

        sqlSb.AppendLine($"CREATE TABLE IF NOT EXISTS \"{table.Name}\" (");
        for (int i = 0; i < table.Fields.Length; i++)
        {
            string comma = (table.Fields.Length > 1) && i < table.Fields.Length - 1 || isPKExists ? "," : "";
            sqlSb.AppendLine($"\"{table.Fields[i].Name}\" {table.Fields[i].Type} {(table.Fields[i].NotNull ? "NOT NULL" : "")} {(table.Fields[i].Unique ? "UNIQUE" : "")}{comma}");
        }

        // PK SQL 생성
        // 인덱스 제약 조건도 생각해야할거 같긴한데 급한건 아니라서 나중에 해도될듯..
        StringBuilder pkStringBuilder = new();
        Field[] pk = table.Fields.Where(field => field.PrimaryKey).ToArray();
        if (isPKExists)
        {
            pkStringBuilder.Append("PRIMARY KEY(");
        }
        for (int i = 0; i < pk.Length; i++)
        {
            string comma = pk.Length > 1 && i < table.Fields.Length - 1 ? "," : "";
            pkStringBuilder.Append($"\"{pk[i].Name}\"{comma}");
            pkStringBuilder.Append($"{(pk[i].AutoIncrement ? " AUTOINCREMENT" : "")}");
        }
        if (isPKExists)
        {
            pkStringBuilder.Append(')');
        }
        sqlSb.AppendLine(pkStringBuilder.ToString());
        sqlSb.AppendLine(");");
        Debug.Log($"{sqlSb}");
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
