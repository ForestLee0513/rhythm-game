using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using BMS;

public class InGameManager : MonoBehaviour
{
    public Pattern patternData = null;
    private static InGameManager instance;
    public static InGameManager Instance { get { return instance; } }

    public float scrollSpeed = 3.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 사운드매니저를 최초 생성
        if (InGameSoundManager.Instance == null)
        {
            GameObject inGameSoundManager = new GameObject("InGameSoundManager");
            inGameSoundManager.AddComponent<InGameSoundManager>();
        }

        // 개발 환경에서 선곡페이지에서 선택하지 않고 씬을 바로 불러올 경우 로컬에 있는 임의의 파일로 지정함.
#if UNITY_EDITOR
        if (GameManager.Instance == null)
        {
            BMSHeaderParser parsedHeaderData = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/[Clue]Random/_random_s4.bms"));
            // BMSHeaderParser parsedHeaderData = new BMSHeaderParser("C:/bmsFiles/[Clue]Random/_random_s4.bms");
            patternData = new BMSMainDataParser(parsedHeaderData.TrackInfo).Pattern;
            InGameSoundManager.Instance.LoadSounds(parsedHeaderData.TrackInfo);
        }
        else
        {
            patternData = new BMSMainDataParser(GameManager.Instance.selectedTrack).Pattern;
            InGameSoundManager.Instance.LoadSounds(GameManager.Instance.selectedTrack);
        }
#elif UNITY_STANDALONE
        patternData = new BMSMainDataParser(GameManager.Instance.selectedTrack).Pattern;
        currentBPM = GameManager.Instance.selectedTrack.TrackInfo.bpm;
        InGameSoundManager.Instance.LoadSounds(GameManager.instance.selectedTrack);
#endif

        InitializeBMSObjectHandler();
    }

    private void InitializeBMSObjectHandler()
    {
        GameObject BGMHandler = new GameObject("BGMHandler");
        BGMHandler.AddComponent<BGMHandler>();

        GameObject BPMHandler = new GameObject("BPMHandler");
        BPMHandler.AddComponent<BPMHandler>();
    }
}
