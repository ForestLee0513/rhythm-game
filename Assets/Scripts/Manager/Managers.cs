using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    private static UIManager _uiManager = new();
    private static ResourceManager _resourceManager = new();
    private static SongInfoDataManager _songInfoDataManager = new("songInfo");
    private static BMSModelManager _bmsModelManager = new();

    public static UIManager UI { get { Init(); return _uiManager; } }
    public static ResourceManager Resource { get { Init(); return _resourceManager; } }
    public static SongInfoDataManager SongInfoDataManager { get { Init(); return _songInfoDataManager; } }
    public static BMSModelManager BMSModelManager { get { Init(); return _bmsModelManager; } }

    private void Start()
    {
        Init();
    }

    private static void Init()
    {
        if (Instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            Instance = Utils.GetOrAddComponent<Managers>(go);
            DontDestroyOnLoad(go);
        }
    }
}
