using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public abstract class SQLiteManager
{
    protected string connectionPath;

    protected SQLiteManager(string dbPath)
    {
        connectionPath = $"URI=file:{dbPath}";
    }

    protected void Validate(List<Table> tables)
    {
        using (SqliteConnection connection = new(connectionPath))
        {
            connection.Open();

            foreach (Table table in tables)
            {
                List<Column> pk = new();

                using (SqliteCommand command = new("SELECT * FROM sqlite_master WHERE name = @name AND type = 'table';", connection))
                {
                    command.Parameters.AddWithValue("@name", table.Name);
                    bool tableExists = false;
                    using (var reader = command.ExecuteReader())
                    {
                        tableExists = reader.HasRows;
                    }

                    if (!tableExists)
                    {
                        StringBuilder sql = new($"CREATE TABLE [{table.Name}] (");
                        bool comma = false;

                        foreach (Column column in table.Columns)
                        {
                            sql.Append(comma ? "," : "")
                                .Append('[').Append(column.Name).Append("] ").Append(column.Type)
                                .Append(column.NotNull == 1 ? " NOT NULL" : "")
                                .Append(!string.IsNullOrEmpty(column.DefaultVal) ? " DEFAULT " + column.DefaultVal : "");

                            comma = true;

                            if (column.Pk == 1)
                            {
                                pk.Add(column);
                            }
                        }

                        if (pk.Count > 0)
                        {
                            sql.Append(", PRIMARY KEY(");
                            comma = false;
                            foreach (var column in pk)
                            {
                                sql.Append(comma ? "," : "").Append(column.Name);
                                comma = true;
                            }
                            sql.Append(")");
                        }

                        sql.Append(");");

                        using (SqliteCommand createCommand = new(sql.ToString(), connection))
                        {
                            createCommand.ExecuteNonQuery();
                        }
                    }
                }

                List<Column> adds = new(table.Columns);

                using (SqliteCommand command = new($"PRAGMA table_info('{table.Name}');", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string existingColumn = reader["name"].ToString();
                            adds.RemoveAll(c => c.Name == existingColumn);
                        }
                    }
                }

                foreach (Column add in adds)
                {
                    string addColumnSql = $"ALTER TABLE {table.Name} ADD COLUMN [{add.Name}] {add.Type} " +
                                       (add.NotNull == 1 ? "NOT NULL" : "") +
                                       (!string.IsNullOrEmpty(add.DefaultVal) ? $" DEFAULT {add.DefaultVal}" : "");

                    using (SqliteCommand addColumnCommand = new(addColumnSql, connection))
                    {
                        addColumnCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    protected void Insert(List<Table> tables, string tableName, object entity)
    {
        Insert(tables, null, tableName, entity);
    }

    protected void Insert(List<Table> tables, SqliteConnection connection, string tableName, object entity)
    {
        Column[] columns = null;
        foreach(Table table in tables)
        {
            if (table.Name == tableName)
            {
                columns = table.Columns;
                break;
            }
        }

        if (columns == null)
        {
            return;
        }

        StringBuilder sql = new("INSERT OR REPLACE INTO " + tableName + " (");
        bool comma = false;
        foreach (Column column in columns)
        {
            sql.Append(comma ? "," :"").Append(column.Name);
            comma = true;
        }
        sql.Append(") VALUES(");

        object[] parameters = new object[columns.Length];
        comma = false;
        for (int i = 0; i < columns.Length; i++)
        {
            sql.Append(comma ? "," : "");
            comma = true;

            try
            {
                PropertyInfo propertyInfo = entity.GetType().GetProperty(columns[i].Name);
                if (propertyInfo != null)
                {
                    object value = propertyInfo.GetValue(entity);
                    parameters[i] = value;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
        }
        sql.Append(");");

        if (connection == null)
        {
            using (SqliteConnection sqliteConnection = new(connectionPath))
            {
                using (var createCommand = new SqliteCommand(sql.ToString(), sqliteConnection))
                {
                    createCommand.ExecuteNonQuery();
                }
            }
        }
        else
        {
            using (connection)
            {
                using (var createCommand = new SqliteCommand(sql.ToString(), connection))
                {
                    createCommand.ExecuteNonQuery();
                }
            }
        }
    }
}

public class Table
{
    public string Name { get; set; }
    public Column[] Columns { get; set; }
}

public class Column
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int NotNull { get; set; }
    public string DefaultVal { get; set; }
    public int Pk { get; set; }
}
