public class SQLiteData
{
    public class Table
    {
        public string Name { get; set; }
        public Field[] Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public SQLiteDefine.Type Type { get; set; }
        public bool NotNull { get; set; }
        public bool PrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        public bool Unique { get; set; }

        public Field(
            string name,
            SQLiteDefine.Type type,
            bool notNull,
            bool primaryKey,
            bool autoIncrement,
            bool unique)
        {
            Name = name;
            Type = type;
            NotNull = notNull;
            // AutoIncrement는 PK가 활성화 됐을 때만 처리되므로 AutoIncrement가 활성화 될때는 PK도 활성화
            PrimaryKey = autoIncrement ? true : primaryKey;
            AutoIncrement = autoIncrement;
            Unique = unique;
        }
    }

    public class Constraint
    {
        public string Field { get; set; }
        public SQLiteDefine.ConstraintType Type { get; set; }
        public string FieldName { get; set; }
    }
}
