using BMSParser;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UISelect : UIPopup
{
    enum Texts
    {

    }

    enum Buttons
    {
        ManageFolerButton
    }

    enum Images
    {

    }

    enum Objects
    {
        SongList,
        SongListViewport,
        SongListContent
    }

    List<FolderInfo.Model> folders;
    List<SongInfo.Model> songs;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindObject(typeof(Objects));

        folders = Managers.SongInfoDataManager.ReadTable<FolderInfo.Model>(FolderInfo.Table.Name);

        foreach (FolderInfo.Model item in folders)
        {
            Debug.Log($"VALUE IS {item.PATH} | {item.FOLDER_NAME}");
        }

        // init folders
        RefreshSongList();
        GetSongs();

        // bind events
        BindEvent(GetButton((int)Buttons.ManageFolerButton).gameObject, OpenFolderManager, UIDefine.UIEvent.Click);

        return true;
    }


    private void RefreshSongList()
    {
        // clear example ui
        Transform parent = GetObject((int)Objects.SongListContent).gameObject.transform;

        foreach (Transform t in parent)
        {
            Managers.Resource.Destroy(t.gameObject);
        }
    }

    private void GetSongs()
    {
        
    }


    private void OpenFolderManager()
    {
        Managers.UI.ShowPopupUI<UIManageFolder>();
    }
}
