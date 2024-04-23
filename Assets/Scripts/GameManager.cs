using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    #endregion
    #region Folder / FileSystem
    public BMSFileSystem BmsFileSystem { get; private set; }
    public int SelectedFolderIndex { get; private set; }
    public List<string> RootPaths { get; private set; }
    public Dictionary<string, List<TrackInfo>> Tracks { get; private set; }
    #endregion
    #region Selected Track
    private string selectedTrackKey = "";
    private int selectedTrackIndex = -1;
    public TrackInfo selectedTrack = null;
    public enum updateSelectedTrackIndexCommandEnum
    {
        Increase,
        Decrease
    }
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        BmsFileSystem = new BMSFileSystem();
        RootPaths = BmsFileSystem.GetRootPaths();
    }

    // 곡 선택 관련 매소드 //
    // 곡 Key 지정
    public void SetTrackKey(string key)
    {
        selectedTrackKey = key;
        selectedTrackIndex = 0;

        // 곡 지정이 됐을 때만 UI 업데이트
        if (selectedTrackKey != "")
        {
            selectedTrack = Tracks[selectedTrackKey][selectedTrackIndex];
            TrackSelectUIManager.Instance.UpdateTrackInfo();
        }
    }

    // 곡 Index 지정
    public void UpdateTrackIndex(updateSelectedTrackIndexCommandEnum command)
    {
        if (selectedTrackKey == "")
        {
            Debug.LogWarning("선택된 곡이 없습니다.");
            return;
        }

        if (command == updateSelectedTrackIndexCommandEnum.Increase)
        {
            selectedTrackIndex++;
        }
        if (command == updateSelectedTrackIndexCommandEnum.Decrease)
        {
            selectedTrackIndex--;
        }

        if (selectedTrackIndex > Tracks[selectedTrackKey].Count - 1)
        {
            selectedTrackIndex = 0;
        }
        else if (selectedTrackIndex < 0)
        {
            selectedTrackIndex = Tracks[selectedTrackKey].Count - 1;
        }

        selectedTrack = Tracks[selectedTrackKey][selectedTrackIndex];
        TrackSelectUIManager.Instance.UpdateTrackInfo();
    }

    // 폴더 관련 매소드 //
    // 폴더 선택
    public void SelectFolder(int index)
    {
        SelectedFolderIndex = index;
        Tracks = BmsFileSystem.ImportFiles(index);
    }

    // 폴더 선택해제
    public void UnSelectFolder()
    {
        SelectedFolderIndex = -1;
        selectedTrackIndex = -1;
        selectedTrackKey = "";
        selectedTrack = null;
        Tracks = null;
    }

    // 키보드 입력 제어 (곡 선택)
    private void Update()
    {
        // 곡 index 감소
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UpdateTrackIndex(updateSelectedTrackIndexCommandEnum.Decrease);
        }
        // 곡 index 증가
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UpdateTrackIndex(updateSelectedTrackIndexCommandEnum.Increase);
        }
    }
}
