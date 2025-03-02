using BMSParser;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEditor;

public class UIManageFolder : UIPopup
{
    enum Texts
    {

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

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        // DB에서 선택된 곡 폴더 불러오기
        // UI 상으로 랜더링 처리
        // 랜더링 된 UI에 클릭 이벤트 추가

        //folders = Managers.SongInfoDataManager.ReadTable<FolderInfo.Model>(FolderInfo.Table.Name);

        //foreach (FolderInfo.Model item in folders)
        //{
        //    Debug.Log($"VALUE IS {item.PATH} | {item.FOLDER_NAME}");
        //}

        GetButton((int)Buttons.OpenFolder).gameObject.BindEvent(OpenFolder);
        GetButton((int)Buttons.RemoveFolder).gameObject.BindEvent(RemoveFolder);

        return true;
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

        // 사용자에 의해 선택이 취소됐는지 확인 - 취소되면 다이얼로그 출력 후 생략
        // 이미 DB에 추가된 경로가 있는지 확인 - 있으면 다이얼로그 출력 후 생략
        // DB 추가
        // UI 상으로 랜더링 처리
        // 랜더링 된 UI에 클릭 이벤트 추가
        List<FolderInfo.Model> existFolderPaths = Managers.SongInfoDataManager.SearchFolderPath<FolderInfo.Model>(selectedPath);

        if (existFolderPaths.Count > 0)
        {
            Debug.Log("경로가 이미 존재함");
            return;
        }

        Debug.Log($"{selectedPath} 폴더 추가");
    }

    private void RemoveFolder()
    {
        // 유니티 UI 상에서 선택됐는지 확인
        // 선택되지 않았다면 예외처리
        // 선택됐으면 DB에서 삭제
        Debug.Log("remove folder");
    }
}
