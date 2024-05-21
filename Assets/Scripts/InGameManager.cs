using System.IO;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public Pattern patternData = null;
    private static InGameManager instance;
    public static InGameManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 개발 환경인 경우에는 로컬에 있는 임의의 파일로 지정함. - 개인 PC에 있는 L9라는 곡의 5B 패턴으로 적용
        #if UNITY_EDITOR
        BMSHeaderParser parsedHeaderData = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/slic_hertz/_slic_hertz_d3.bme"));
        patternData = new BMSMainDataParser(parsedHeaderData.TrackInfo).Pattern;
        #elif UNITY_STANDALONE
        patternData = new BMSMainDataParser(GameManager.Instance.selectedTrack).Pattern;
        currentBPM = GameManager.Instance.selectedTrack.TrackInfo.bpm;
        #endif
    }
}
