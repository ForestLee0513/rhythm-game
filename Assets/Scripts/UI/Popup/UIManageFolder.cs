using System.Collections.Generic;
using UnityEngine;
using SFB;

public class UIManageFolder : UIPopup
{
    enum Texts
    {

    }

    enum Objects
    {
        FolderListContent
    }

    enum Buttons
    {
        OpenFolder,
        RemoveFolder
    }

    enum Images
    {

    }

    List<FolderInfo.Model> folders;
    int selectedFolderIndex = -1;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindObject(typeof(Objects));

        GetButton((int)Buttons.OpenFolder).gameObject.BindEvent(OpenFolder);
        GetButton((int)Buttons.RemoveFolder).gameObject.BindEvent(RemoveFolder);

        RefreshFolders();

        return true;
    }

    private void RefreshFolders()
    {
        // clear example ui
        Transform parent = GetObject((int)Objects.FolderListContent).gameObject.transform;
        
        foreach (Transform t in parent)
        {
            Managers.Resource.Destroy(t.gameObject);
        }

        // append ui
        folders?.Clear();
        folders = Managers.SongInfoDataManager.ReadTable<FolderInfo.Model>(FolderInfo.Table.Name);

        foreach (FolderInfo.Model folderItem in folders)
        {
            var folderItemObject = Managers.UI.MakeSubItem<UI_FolderListItem>(parent.transform);
            folderItemObject.SetInfo(folderItem.FOLDER_NAME, folderItem.PATH);
        }
    }

    private void OpenFolder()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("폴더 선택", "", false);

        if (paths.Length == 0)
        {
            Debug.Log("user cancel");
            return;
        }

        string selectedPath = paths[0];

        List<FolderInfo.Model> existFolderPaths = Managers.SongInfoDataManager.SearchFolderPath<FolderInfo.Model>(selectedPath);

        if (existFolderPaths.Count > 0)
        {
            Debug.Log("경로가 이미 존재함");
            return;
        }

        if (Managers.SongInfoDataManager.PushFolder(selectedPath))
        {
            Debug.Log("파일 추가 완료");
            RefreshFolders();
        }
    }

    private void RemoveFolder()
    {
        if (selectedFolderIndex == -1)
            return;

        // 유니티 UI 상에서 선택됐는지 확인
        // 선택되지 않았다면 예외처리
        // 선택됐으면 DB에서 삭제
        Debug.Log("remove folder");
    }

}
