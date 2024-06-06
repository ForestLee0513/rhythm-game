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

        // 개발 환경인 경우에는 로컬에 있는 임의의 파일로 지정함.
        #if UNITY_EDITOR
        BMSHeaderParser parsedHeaderData = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/Merry Christmas Mr. (by sasakure.UK)/Merry Christmas Mr.Lawrence.bme"));
        patternData = new BMSMainDataParser(parsedHeaderData.TrackInfo).Pattern;
        InGameSoundManager.Instance.LoadSounds(parsedHeaderData.TrackInfo);
        // 메트로놈이 없는 경우 최초 생성
        if (Metronome.Instance == null)
        {
            GameObject metronome = new GameObject("Metronome");
            Metronome metronomeComponent = metronome.AddComponent<Metronome>();
            double beat = patternData.beatMeasureLengthTable.ContainsKey(0) ? patternData.beatMeasureLengthTable[0] : 1;

            metronomeComponent.Init(parsedHeaderData.TrackInfo.bpm, patternData.bar, beat);
        }
        #elif UNITY_STANDALONE
        patternData = new BMSMainDataParser(GameManager.Instance.selectedTrack).Pattern;
        currentBPM = GameManager.Instance.selectedTrack.TrackInfo.bpm;
        InGameSoundManager.Instance.LoadSounds(GameManager.instance.selectedTrack);
        // 메트로놈이 없는 경우 최초 생성
        if (Metronome.Instance == null)
        {
            GameObject metronome = new GameObject("Metronome");
            Metronome metronomeComponent = metronome.AddComponent<Metronome>();
            double beat = patternData.beatMeasureLengthTable.ContainsKey(0) ? patternData.beatMeasureLengthTable[0] : 1;

            metronomeComponent.Init(parsedHeaderData.TrackInfo.bpm, patternData.bar, beat);
        }
        #endif

        // BMS 오브젝트 관리자 생성
        GameObject bmsObjectHandler = new GameObject("BMSObjectHandler");
        bmsObjectHandler.AddComponent<BMSObjectHandler>();
    }
}
