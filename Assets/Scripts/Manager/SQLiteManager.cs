using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.Search;
using UnityEngine;
using static SQLiteData;

public abstract class SQLiteManager
{
    protected string connectionPath;
    protected string filePath;
    protected SQLiteManager(string fileName)
    {
        filePath = $"{Application.persistentDataPath}/{fileName}.db";
        connectionPath = $"URI=file:{Application.persistentDataPath}/{fileName}.db";
    }

    public void Init()
    {
        CreateFileIfNotExists();
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
        bool isPKExists = false;
        int autoIncrementCount = 0;
        StringBuilder sqlSb = new();
            
        // 추가할 필드가 없으면 sql 추가 중단
        if (table.Fields.Length == 0)
        {
            Debug.LogWarning($"{table.Name} 테이블은 필드값이 없어 생략합니다.");
            return;
        }

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
            sqlSb.AppendLine($"\"{table.Fields[i].Name}\" {table.Fields[i].Type}{(table.Fields[i].NotNull ? " NOT NULL" : "")}{(table.Fields[i].Unique ? " UNIQUE" : "")}{comma}");
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
        RunQuery(sqlSb.ToString());
    }

    // 여기 예외처리 해야하는데..
    // UNIQUE 여부 확인
    public void RunQuery(string query)
    {
        using SqliteConnection connection = new(connectionPath);
        connection.Open();
        IDbCommand dbCommand = connection.CreateCommand();
        dbCommand.CommandText = query;
        IDataReader reader = dbCommand.ExecuteReader();

        reader.Close();
    }

    public bool IsTableExists(string tableName)
    {
        using SqliteConnection connection = new(connectionPath);
        connection.Open();
        var sql = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";
        if (connection.State == ConnectionState.Open)
        {
            SqliteCommand command = new SqliteCommand(sql, connection);
            SqliteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }

            reader.Close();
            return false;
        }
        else
        {
            throw new System.ArgumentException("Data.ConnectionState must be open");
        }
    }

    public List<T> ReadTable<T>(string tableName) where T : new()
    {
        using SqliteConnection connection = new(connectionPath);
        connection.Open();
        IDbCommand dbCommand = connection.CreateCommand();
        dbCommand.CommandText = $"SELECT * FROM {tableName}";
        IDataReader reader = dbCommand.ExecuteReader();

        List<T> values = new();
        while (reader.Read())
        {
            T value = new();
            var properties = typeof(T).GetFields();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                var field = properties.FirstOrDefault(p =>
                    p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                if (field != null)
                {
                    var dbValue = reader.GetValue(i);
                    if (dbValue != DBNull.Value)
                    {
                        field.SetValue(value, Convert.ChangeType(dbValue, field.FieldType));
                    }
                }
            }

            values.Add(value);
        }

        reader.Close();
        return values;
    }
}
