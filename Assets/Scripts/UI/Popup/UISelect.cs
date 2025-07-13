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

        RefreshSongList();
        GetSongs();

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
}
