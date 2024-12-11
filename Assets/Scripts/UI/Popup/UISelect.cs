using BMSParser;
using System.Collections.Generic;
using UnityEngine;

public class UISelect : UIPopup
{
    enum Texts
    {

    }

    enum Buttons
    {

    }

    enum Images
    {

    }

    List<FolderInfo.Model> folders;

    public override bool Init()     
    {
        if (base.Init() == false)
            return false;

        // ReadTable 선언 시 테스트
        folders = Managers.SongInfoDataManager.ReadTable<FolderInfo.Model>(FolderInfo.Table.Name);

        foreach (FolderInfo.Model item in folders)
        {
            Debug.Log($"VALUE IS {item.PATH} | {item.FOLDER_NAME}");
        }

        Debug.Log("== UPDATE TABLE STARTS FROM HERE ==");
        Managers.SongInfoDataManager.RunQuery($"INSERT INTO {FolderInfo.Table.Name} VALUES('D:\\bms\\6K U_E FULL PACK 1.13', '6K U_E FULL PACK 1.13')");
        folders = Managers.SongInfoDataManager.ReadTable<FolderInfo.Model>(FolderInfo.Table.Name);
        foreach (FolderInfo.Model item in folders)
        {
            Debug.Log($"VALUE IS {item.PATH} | {item.FOLDER_NAME}");
        }

        return true;
    }
}
