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
            Debug.LogError("�ش� ���� ���� ���� �����ϴ�.");
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
        // �ش� bms������ �θ� �ٸ��ٸ� �ٸ� ������ �����Ͽ� ���ҽ� ����
        if (previousTrack == null || Directory.GetParent(previousTrack.path).FullName != Directory.GetParent(selectedTrack.path).FullName)
        {
            // ���� ���� ����� ĳ�� ���� �� ����� ���ҽ� ����
            SoundManager.Instance.ClearAudioCache();
            SoundManager.Instance.LoadSounds(selectedTrack);

            // �̹��� ĳ�� ���� �� �̹��� ���ҽ� ����
            // ...
            previousTrack = selectedTrack;
        }
    }
}
