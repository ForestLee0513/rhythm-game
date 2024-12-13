using BMSParser;
using System.Collections.Generic;
using UnityEngine;

public class UIManageFolder : UIPopup
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

        folders = Managers.SongInfoDataManager.ReadTable<FolderInfo.Model>(FolderInfo.Table.Name);

        foreach (FolderInfo.Model item in folders)
        {
            Debug.Log($"VALUE IS {item.PATH} | {item.FOLDER_NAME}");
        }

        return true;
    }
}
