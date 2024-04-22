using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal.Converters;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TrackSelectUIManager : MonoBehaviour
{
    #region Singleton 
    private static TrackSelectUIManager instance;
    public static TrackSelectUIManager Instance { get { return instance; } }
    #endregion

    #region UI GameObjects
    [SerializeField]
    GameObject ListItemPrefab;
    GameObject trackList;
    GameObject backButton;
    #endregion

    // Initialize //
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (ListItemPrefab == null) 
        {
            Debug.LogError("List item prefab is not imported.");
            return;
        }

        trackList = transform.Find("FolderScrollView/Viewport/Content").gameObject;
        backButton = transform.Find("BackButton").gameObject;

        backButton.SetActive(false);
        // Initialize Lists
        for (int i = 0; i < GameManager.Instance.RootPaths.Count; ++i)
        {
            // delegate 추가용 index 지역 변수
            int index = i;
            AppendToList(new DirectoryInfo(GameManager.Instance.RootPaths[index]).Name, GameManager.Instance.RootPaths[index], () => { SelectFolder(index); });
        }
    }

    private void AppendToList(string title, string description, UnityAction func)
    {
        GameObject item = Instantiate(ListItemPrefab);
        EventTrigger trigger = item.GetComponent<EventTrigger>();

        // set Text
        item.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        item.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = description;

        // add Event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { func(); });
        trigger.triggers.Add(entry);

        // set Parent
        item.transform.SetParent(trackList.transform, false);
    }

    private void SelectFolder(int index)
    {
        // 이거 index를 전달할 방법을 찾아야한다...
        Debug.Log(index);
        Debug.Log("SelectFolder");
    }
}
