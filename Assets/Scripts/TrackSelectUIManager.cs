using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal.Converters;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    GameObject trackListContent;
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

        trackList = transform.Find("FolderScrollView").gameObject;
        trackListContent = trackList.transform.Find("Viewport/Content").gameObject;
        backButton = transform.Find("BackButton").gameObject;

        SetListToRootPaths();
    }

    private void AppendToList(string title, string description, UnityAction func)
    {
        // append list
        GameObject item = Instantiate(ListItemPrefab);
        EventTrigger trigger = item.GetComponent<EventTrigger>();

        // set Text
        item.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        item.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = description;

        // add Event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(
            (eventData) => 
            {
                func(); 
            });
        trigger.triggers.Add(entry);

        // set Parent
        item.transform.SetParent(trackListContent.transform, false);
        item.GetComponent<ListItem>().ParentScrollRect = trackList.GetComponent<ScrollRect>();
    }

    private void SelectFolder(int index)
    {
        GameManager.Instance.SelectFolder(index);

        backButton.SetActive(true);
        foreach (Transform child in trackListContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string trackKey in GameManager.Instance.Tracks.Keys)
        {
            AppendToList(
                GameManager.Instance.Tracks[trackKey][0].title, 
                GameManager.Instance.RootPaths[index], 
                () => 
                { 
                    Debug.Log("Track Select" + "/" + GameManager.Instance.Tracks[trackKey][0].title); 
                });
        }
    }

    public void SetListToRootPaths()
    {
        backButton.SetActive(false);
        GameManager.Instance.UnSelectFolder();

        foreach (Transform child in trackListContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < GameManager.Instance.RootPaths.Count; ++i)
        {
            // delegate �߰��� index ���� ����
            int index = i;
            AppendToList(
                new DirectoryInfo(GameManager.Instance.RootPaths[index]).Name,
                GameManager.Instance.RootPaths[index],
                () =>
                {
                    SelectFolder(index);
                });
        }
    }
}
