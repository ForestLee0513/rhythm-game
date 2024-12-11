using static SQLiteData;

public class FolderInfo
{
    public static Table Table { get; set; } = new Table()
    {
        Name = "FolderInfo",
        Fields = new Field[] {
            new("PATH", SQLiteDefine.Type.TEXT, true, true, false, true),
            new("FOLDER_NAME", SQLiteDefine.Type.TEXT, true, false, false, false),
        },
    };

    // 모델 선언 시에는 ReadTable에서 바로 배열로 반환할 수 있도록 선언했던 필드명과 동일하게 선언
    public class Model
    {
        public string PATH;
        public string FOLDER_NAME;
    }
}
