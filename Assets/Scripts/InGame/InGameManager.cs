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
        BMSHeaderParser parsedHeaderData = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/L9 (by paraoka)/9_5.bms"));
        // BMSHeaderParser parsedHeaderData = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/Aleph-0 (by LeaF)/_7ANOTHER.bms"));
        patternData = new BMSMainDataParser(parsedHeaderData.TrackInfo).Pattern;
        InGameSoundManager.Instance.LoadSounds(parsedHeaderData.TrackInfo);
        // 메트로놈이 없는 경우 최초 생성
        if (Metronome.Instance == null)
        {
            GameObject metronome = new GameObject("Metronome");
            Metronome metronomeComponent = metronome.AddComponent<Metronome>();
            double beat = patternData.beatMeasureLengthTable.ContainsKey(0) ? patternData.beatMeasureLengthTable[0] : 1;

            metronomeComponent.Init(parsedHeaderData.TrackInfo.bpm, patternData.totalBarCount, beat);
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

            metronomeComponent.Init(parsedHeaderData.TrackInfo.bpm, patternData.totalBarCount, beat);
        }
        #endif

        // BGM 제어 컴포넌트 추가
        GameObject BGMHandler = new GameObject("BGMHandler");
        BGMHandler.AddComponent<BGMHandler>();
    }
}
