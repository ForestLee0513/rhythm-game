using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private TrackInfo selectedTrack;
    private TrackInfo previousTrack;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SelectTrack(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectTrack(BMSFileSystem.TrackInfoList.IndexOf(selectedTrack) + 1);
        }
    }

    public void SelectTrack(int index)
    {
        if (BMSFileSystem.TrackInfoList.Count <= index)
        {
            Debug.LogError("해당 범위 내에 곡이 없습니다.");
            return;
        }

        selectedTrack = BMSFileSystem.TrackInfoList[index];
        if (selectedTrack != null)
        {
            StartCoroutine("SwapSongResource");
        }
    }

    [System.Obsolete]
    IEnumerator SwapSongResource()
    {
        yield return null;
        // 해당 bms파일의 부모가 다르다면 다른 곡으로 간주하여 리소스 변경
        if (previousTrack == null || Directory.GetParent(previousTrack.path).FullName != Directory.GetParent(selectedTrack.path).FullName)
        {
            // 기존 곡의 오디오 캐시 삭제 및 오디오 리소스 변경
            SoundManager.Instance.ClearAudioCache();
            SoundManager.Instance.LoadSounds(selectedTrack);

            // 이미지 캐시 삭제 및 이미지 리소스 변경
            // ...
            previousTrack = selectedTrack;
        }
    }
}
