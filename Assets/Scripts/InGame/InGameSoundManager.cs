using System.IO;
using BMS;
using FMODUnity;
using UnityEngine;

public class InGameSoundManager : MonoBehaviour
{
    private static InGameSoundManager instance;
    public static InGameSoundManager Instance { get { return instance; } }
    // 게임 내에서 사용할 악기 사운드
    private FMOD.ChannelGroup trackSoundChannelGroup;
    private FMOD.Sound[] trackSounds;
    private FMOD.Channel trackSoundChannel;

    private string[] extensions = { ".wav", ".mp3", ".ogg" };
    private string selectedExtension;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void LoadSounds(TrackInfo trackInfo)
    {
        trackSoundChannelGroup = new FMOD.ChannelGroup();
        trackSoundChannel = new FMOD.Channel();
        trackSounds = new FMOD.Sound[1296];

        foreach (int trackSoundKey in trackInfo.audioFileNames.Keys)
        {
            foreach (string extension in extensions)
            {
                if (File.Exists($"{trackInfo.audioFileNames[trackSoundKey]}{extension}"))
                {
                    selectedExtension = extension;
                    break;
                }
            }

            string path = $"{trackInfo.audioFileNames[trackSoundKey]}{selectedExtension}";

            FMODUnity.RuntimeManager.CoreSystem.createSound(path, FMOD.MODE.CREATESAMPLE, out trackSounds[trackSoundKey]);
        }
        trackSoundChannel.setChannelGroup(trackSoundChannelGroup);
    }

    public void PlaySound(int channelKey)
    {
        FMOD.RESULT result = FMODUnity.RuntimeManager.CoreSystem.playSound(trackSounds[channelKey], trackSoundChannelGroup, false, out trackSoundChannel);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Failed to play sound: " + result);
            return;
        }

        trackSoundChannel.getPaused(out bool isPaused);
        if (isPaused)
        {
            trackSoundChannel.setPaused(false);
        }
        else
        {
            trackSoundChannel.stop();
            FMODUnity.RuntimeManager.CoreSystem.playSound(trackSounds[channelKey], trackSoundChannelGroup, false, out trackSoundChannel);
        }
    }

    void OnDestroy() {
        foreach (var sound in trackSounds) {
            sound.release();
        }
        trackSounds = null;
    }
}
