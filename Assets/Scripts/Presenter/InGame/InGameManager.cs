using BMS;
using System.IO;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public Pattern patternData = null;
    private static InGameManager instance;
    public static InGameManager Instance { get { return instance; } }
    public TrackInfo selectedTrack;

    [SerializeField]
    TimelineSubject timelineSubject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 사운드매니저를 먼저 생성
        if (InGameSoundManager.Instance == null)
        {
            GameObject inGameSoundManager = new GameObject("InGameSoundManager");
            inGameSoundManager.AddComponent<InGameSoundManager>();
        }

        // 개발 환경에서 선곡페이지에서 선택하지 않고 씬을 바로 불러올 경우 로컬에 있는 임의의 파일로 지정함.
#if UNITY_EDITOR
        if (GameManager.Instance == null)
        {
            //selectedTrack = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/[モリモリあつし] MilK/_MilK_Aery.bms")).TrackInfo;
            selectedTrack = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/[Clue]Random/_random_s4.bms")).TrackInfo;
            //selectedTrack = new BMSHeaderParser("C:/bmsFiles/Merry Christmas Mr. (by sasakure.UK)/mcml(4-7uet).bme").TrackInfo;
            //selectedTrack = new BMSHeaderParser("C:/bmsFiles/Aleph-0 (by LeaF)/_7INSANE.bms").TrackInfo;
            patternData = new BMSMainDataParser(selectedTrack).Pattern;
        }
        else
        {
            selectedTrack = GameManager.Instance.selectedTrack;
            patternData = new BMSMainDataParser(selectedTrack).Pattern;
            
        }
#elif UNITY_STANDALONE
        selectedTrack = GameManager.Instance.selectedTrack;
        patternData = new BMSMainDataParser(selectedTrack).Pattern;
#endif

        InGameSoundManager.Instance.LoadSounds(selectedTrack);
    }

    private void Start()
    {
        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.LoadBGAAssets(selectedTrack.imageFileNames, BGASequence.BGAFlagState.Image);
            InGameUIManager.Instance.LoadBGAAssets(selectedTrack.videoFileNames, BGASequence.BGAFlagState.Video);
        }

        timelineSubject.StartTime();
    }
}
