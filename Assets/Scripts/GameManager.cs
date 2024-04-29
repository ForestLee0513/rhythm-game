using System.Collections.Generic;
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

    // �� ���� ���� �żҵ� //
    // �� Key ����
    public void SetTrackKey(string key)
    {
        selectedTrackKey = key;
        selectedTrackIndex = 0;


        selectedTrack = Tracks[selectedTrackKey][selectedTrackIndex];
        TrackSelectUIManager.Instance.UpdateTrackInfo();
    }

    // �� Index ����
    public void UpdateTrackIndex(updateSelectedTrackIndexCommandEnum command)
    {
        if (selectedTrackKey == "")
        {
            TrackSelectUIManager.Instance.UpdateTrackInfo();
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

    // ���� ���� �żҵ� //
    // ���� ����
    public void SelectFolder(int index)
    {
        SelectedFolderIndex = index;
        Tracks = BmsFileSystem.ImportFiles(index);
    }

    // ���� ��������
    public void UnSelectFolder()
    {
        SelectedFolderIndex = -1;
        selectedTrackIndex = -1;
        selectedTrackKey = "";
        selectedTrack = null;
        Tracks = null;
        TrackSelectUIManager.Instance.UpdateTrackInfo();
    }

    // Ű���� �Է� ���� (�� ����)
    private void Update()
    {
        // �� index ����
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UpdateTrackIndex(updateSelectedTrackIndexCommandEnum.Decrease);
        }
        // �� index ����
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UpdateTrackIndex(updateSelectedTrackIndexCommandEnum.Increase);
        }
    }
}
