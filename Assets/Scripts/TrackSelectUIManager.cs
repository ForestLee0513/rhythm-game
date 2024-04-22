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
    GameObject listTitle;
    GameObject listDescription;
    string listTitleInitializedText;
    string listDescriptionInitializedText;
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
        listTitle = transform.Find("Title").gameObject;
        listDescription = transform.Find("Description").gameObject;
        listTitleInitializedText = listTitle.GetComponent<TextMeshProUGUI>().text;
        listDescriptionInitializedText = listDescription.GetComponent<TextMeshProUGUI>().text;


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

        listTitle.GetComponent<TextMeshProUGUI>().text = new DirectoryInfo(GameManager.Instance.RootPaths[index]).Name;
        listDescription.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.RootPaths[index];

        foreach (string trackKey in GameManager.Instance.Tracks.Keys)
        {
            AppendToList(
                trackKey, 
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

        listTitle.GetComponent<TextMeshProUGUI>().text = listTitleInitializedText;
        listDescription.GetComponent<TextMeshProUGUI>().text = listDescriptionInitializedText;

        for (int i = 0; i < GameManager.Instance.RootPaths.Count; ++i)
        {
            // delegate 추가용 index 지역 변수
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
