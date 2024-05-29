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
    
    // 메트로놈 //
    private double bpm = 0.0f;
    // 현재 메트로놈 마디
    private int currentBarCount = 0;
    // 마지막 메트로놈 마디
    private int lastBarCount = 0;
    // 다음 메트로놈 틱 시간
    private double nextTickTime = 0;
    // 틱 간격 시간
    private double tickInterval = 0;
    // 틱 횟수
    private int tickCount = 0;
    // 기믹 //
    // 정지 기믹 활성화 여부
    private bool isStopGimmikEnabled = false;

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

        // 개발 환경인 경우에는 로컬에 있는 임의의 파일로 지정함.
        #if UNITY_EDITOR
        BMSHeaderParser parsedHeaderData = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/Merry Christmas Mr. (by sasakure.UK)/Merry Christmas Mr.Lawrence.bme"));
        patternData = new BMSMainDataParser(parsedHeaderData.TrackInfo).Pattern;
        InGameSoundManager.Instance.LoadSounds(parsedHeaderData.TrackInfo);
        bpm = parsedHeaderData.TrackInfo.bpm;
        #elif UNITY_STANDALONE
        patternData = new BMSMainDataParser(GameManager.Instance.selectedTrack).Pattern;
        currentBPM = GameManager.Instance.selectedTrack.TrackInfo.bpm;
        InGameSoundManager.Instance.LoadSounds(GameManager.instance.selectedTrack);
        bpm = GameManager.Instance.selectedTrack.TrackInfo.bpm;
        #endif

        tickInterval = 60 / bpm;
        nextTickTime = Time.time + tickInterval;
        lastBarCount = patternData.bar;

    }

    private void Update()
    {
        Mertonome();
    }

    // 기믹 관련 //
    // 변속
    public void SetBpm(double newBpm)
    {
        bpm = newBpm;
    }
    
    // 정지기믹 활성화
    public void ToggleStopGimmik()
    {
        isStopGimmikEnabled = !isStopGimmikEnabled;
    }

    public void ToggleStopGimmik(bool state)
    {
        isStopGimmikEnabled = state;
    }

    // 메트로놈
    private void Mertonome()
    {
        if (currentBarCount == lastBarCount)
        {
            return;
        }

        if (Time.time >= nextTickTime)
        {
            // 다음 틱 시간 계산
            nextTickTime += tickInterval;

            // 틱 카운트 증가
            tickCount++;
            Debug.Log("Tick");

            // 4 틱마다 바 소리 재생 (4/4 박자 기준)
            if (tickCount % 4 == 0)
            {
                currentBarCount++;
                Debug.Log("Bar");
            }
        }
    }
}
