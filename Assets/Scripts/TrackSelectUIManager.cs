using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    #region UI GameObjects - List
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
    #region UI GameObjects - TrackInfo
    GameObject trackInfoContainer;
    GameObject trackInfoJacketImage;
    GameObject trackInfo;
    GameObject title;
    GameObject artist;
    GameObject level;
    GameObject genre;
    GameObject bpm;
    GameObject score;
    GameObject judge;
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

        // Initialize GameObjects of List
        trackList = transform.Find("FolderScrollView").gameObject;
        trackListContent = trackList.transform.Find("Viewport/Content").gameObject;
        backButton = transform.Find("BackButton").gameObject;
        listTitle = transform.Find("Title").gameObject;
        listDescription = transform.Find("Description").gameObject;
        listTitleInitializedText = listTitle.GetComponent<TextMeshProUGUI>().text;
        listDescriptionInitializedText = listDescription.GetComponent<TextMeshProUGUI>().text;

        // Initialize GameObjects of Track Info
        trackInfoContainer = transform.Find("TrackInfoContainer").gameObject;
        trackInfoJacketImage = trackInfoContainer.transform.Find("TrackInfoJacketImage").gameObject;
        trackInfo = trackInfoContainer.transform.Find("TrackInfo").gameObject;

        title = trackInfo.transform.Find("Title/Text").gameObject;
        artist = trackInfo.transform.Find("Artist/Text").gameObject;
        level = trackInfo.transform.Find("Level/Text").gameObject;
        genre = trackInfo.transform.Find("Genre/Text").gameObject;
        bpm = trackInfo.transform.Find("BPM/Text").gameObject;
        score = trackInfo.transform.Find("Score/Text").gameObject;
        judge = trackInfo.transform.Find("Judge/Text").gameObject;

        SetListToRootPaths();
        trackInfo.SetActive(false);
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
                    GameManager.Instance.SetTrackKey(trackKey);
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

    public void UpdateTrackInfo()
    {
        if (GameManager.Instance.selectedTrack == null)
        {
            trackInfo.SetActive(false);
            return;
        }

        if (trackInfo.activeSelf == false)
        {
            trackInfo.SetActive(true);
        }

        // bpm 범위 출력을 위한 단순 정렬 실제 패턴에는 사용안함
        float[] bpmRangeToArr = GameManager.Instance.selectedTrack.bpmTable.Values.ToArray();
        string bpmRangeResult = "";
        if (bpmRangeToArr.Length > 0)
        {
            Array.Sort(bpmRangeToArr, (a, b) => (a > b) ? 1 : -1);

            if (bpmRangeToArr.Length == 1)
            {
                bpmRangeResult = $" ({GameManager.Instance.selectedTrack.bpm} ~ {bpmRangeToArr[0]})";
            }
            else
            {
                bpmRangeResult = $" ({bpmRangeToArr[0]} ~ {bpmRangeToArr[bpmRangeToArr.Length - 1]})";
            }
        }

        title.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.title;
        artist.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.artist;
        level.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.playLevel.ToString();
        genre.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.genre;
        bpm.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.bpm.ToString() + bpmRangeResult;
    }
}
