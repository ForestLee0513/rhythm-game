using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    private static UIManager _uiManager = new();
    private static ResourceManager _resourceManager = new();

    public static UIManager UI { get { Init(); return _uiManager; } }
    public static ResourceManager Resource { get { Init(); return _resourceManager; } }
    
    // Data Managers //


    void Start()
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
