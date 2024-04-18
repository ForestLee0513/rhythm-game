using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows.Speech;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();
    private string[] extensions = new string[] { ".ogg", ".wav", ".mp3" };
    private AudioType[] extensionTypes = new AudioType[] { AudioType.OGGVORBIS, AudioType.WAV, AudioType.MPEG };

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
        audioSource = GetComponent<AudioSource>();
    }

    [System.Obsolete]
    public void LoadSounds(TrackInfo track)
    {
        StartCoroutine(LoadSound(track));
    }

    private IEnumerator LoadSound(TrackInfo track)
    {
        int extensionFailCount = 0;
        foreach (string audioHexKey in track.audioFileNames.Keys)
        {
            string url = track.audioFileNames[audioHexKey];
            UnityWebRequest www = null;
            extensionFailCount = 0;
            AudioType type = AudioType.OGGVORBIS;
            do
            {
                if (File.Exists(System.Web.HttpUtility.UrlDecode(url) + extensions[extensionFailCount]))
                {
                    url = url + extensions[extensionFailCount];
                    type = extensionTypes[extensionFailCount];
                    break;
                }
                ++extensionFailCount;
            }
            while (extensionFailCount < extensions.Length - 1);

            www = UnityWebRequestMultimedia.GetAudioClip("file://" + url, type);
            yield return www.SendWebRequest();

            Debug.Log(url);
            Debug.Log(www.responseCode);

            if (www.downloadHandler.data.Length != 0)
            {
                AudioClip c = DownloadHandlerAudioClip.GetContent(www);
                c.LoadAudioData();
                audioClipCache.Add(audioHexKey, c);
            }
            else
            {
                Debug.LogWarning($"Failed to read sound data : {www.url}");
            }
        }
    }

    public void PlayBaseSound()
    {
        //audioSource.outputAudioMixerGroup.
    }

    public void PlayKeySound()
    {

    }

    public void ClearAudioCache()
    {
        audioClipCache.Clear();
    }
}
