using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_FolderListItem : UIBase
{

    enum Texts
    {
        FolderName,
        Path
    }

    enum Images
    {
        Background
    }

    private string _folderName;
    private string _path;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindImage(typeof(Images));

        RefreshUI();

        return true;
    }

    public void SetInfo(string folderName, string path)
    {
        _folderName = folderName;
        _path = path;
    }

    private void RefreshUI()
    {
        GetText((int)Texts.FolderName).text = _folderName;
        GetText((int)Texts.Path).text = _path;
    }
}
