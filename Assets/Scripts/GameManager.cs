using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    public BMSFileSystem BmsFileSystem { get; private set; }
    public int SelectedFolderIndex { get; private set; }
    public List<string> RootPaths { get; private set; }
    public Dictionary<string, List<TrackInfo>> Tracks { get; private set; }

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
        Tracks = null;
    }
}
