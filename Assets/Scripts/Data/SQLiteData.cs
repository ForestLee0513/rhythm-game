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
    }

    public class Constraint
    {
        public string Field { get; set; }
        public SQLiteDefine.ConstraintType Type { get; set; }
        public string FieldName { get; set; }
    }
}
