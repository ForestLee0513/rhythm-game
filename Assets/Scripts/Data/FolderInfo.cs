using static SQLiteData;

public static class FolderInfo
{
    public static Table Table { get; set; } = new Table()
    {
        Name = "FolderInfo",
        Fields = new Field[] {
            new("PATH", SQLiteDefine.Type.TEXT, true, true, false, true),
            new("FOLDER_NAME", SQLiteDefine.Type.TEXT, true, false, false, false),
        },
    };
}
