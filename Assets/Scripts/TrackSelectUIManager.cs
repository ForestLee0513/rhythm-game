using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using B83.Image.BMP;
using static GameManager;


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

        // Initialize UI Elements //
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

    private void Update()
    {
        // 곡 선택
        if (Input.GetKeyDown(KeyCode.Return))
        {
            MoveToInGame();
        }

        // 게임 종료
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitController();
        }

        // 곡 난이도 변경 //
        // index 감소
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.Instance.UpdateTrackIndex(updateSelectedTrackIndexCommandEnum.Decrease);
        }
        // index 증가
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.Instance.UpdateTrackIndex(updateSelectedTrackIndexCommandEnum.Increase);
        }
    }

    private void MoveToInGame()
    {
        // 곡
        if (GameManager.Instance.selectedTrack == null)
        {
            Debug.LogError("곡이 선택되지 않아 이동하지 않습니다..");
            return;
        }

        SceneLoader.LoadScene("InGame");
    }    

    private void ExitController()
    {
        // 폴더가 선택됐을 때 실행
        if (GameManager.Instance.SelectedFolderIndex >= 0)
        {
            Debug.Log("폴더선택 해제");
            SetListToRootPaths();
        }
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

        // 경로가 존재하지 않을 경우 선택을 시도했던 경로 선택 취소 및 에러 출력 (추후 모달도 출력 예정.)
        if (GameManager.Instance.Tracks == null)
        {
            Debug.LogError("해당 경로가 존재하지 않습니다.");
            GameManager.Instance.UnSelectFolder();
            return;
        }

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
                $"Total: {GameManager.Instance.Tracks[trackKey].Count}", 
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
            // delegate에 index를 지정하기 위한 지역변수
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

        // bpm ���� ����� ���� �ܼ� ���� ���� ���Ͽ��� ������
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

        Texture2D tex = null;
        byte[] fileData;

        // Load image //
        // Load bmp
        if (Path.GetExtension(GameManager.Instance.selectedTrack.stageFile) == ".bmp")
        {
            BMPLoader loader = new BMPLoader();
            BMPImage img = loader.LoadBMP(GameManager.Instance.selectedTrack.stageFile);
            tex = img.ToTexture2D();
            trackInfoJacketImage.GetComponent<RawImage>().texture = tex;
        }
        else
        {
            // Load png, jpg ...
            if (File.Exists(GameManager.Instance.selectedTrack.stageFile))
            {
                fileData = File.ReadAllBytes(GameManager.Instance.selectedTrack.stageFile);
                
                tex = new Texture2D(1, 1);
                tex.LoadImage(fileData);
                float ratio = (float)tex.width / tex.height;
                trackInfoJacketImage.GetComponent<AspectRatioFitter>().aspectRatio = ratio;
                trackInfoJacketImage.GetComponent<RawImage>().texture = tex;
            }
        }
        title.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.title;
        artist.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.artist;
        level.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.playLevel.ToString();
        genre.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.genre;
        bpm.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.selectedTrack.bpm.ToString() + bpmRangeResult;
    }
}
