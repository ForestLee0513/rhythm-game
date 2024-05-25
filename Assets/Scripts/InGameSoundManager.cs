using System;
using BMS;
using UnityEngine;

public class InGameSoundManager : MonoBehaviour
{
    private static InGameSoundManager instance;
    public static InGameSoundManager Instance { get { return instance; } }
    // 게임 내에서 사용할 악기 사운드
    private FMOD.ChannelGroup trackSoundChannelGroup;
    private FMOD.Sound[] trackSounds;
    private FMOD.Channel[] trackSoundChannels;

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
        trackSoundChannels = new FMOD.Channel[1295];
        trackSounds = new FMOD.Sound[1295];

        foreach (int trackSoundKey in trackInfo.audioFileNames.Keys)
        {
            string path = trackInfo.audioFileNames[trackSoundKey];

            FMODUnity.RuntimeManager.CoreSystem.createSound(path, FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out trackSounds[trackSoundKey]);
            trackSoundChannels[trackSoundKey].setChannelGroup(trackSoundChannelGroup);
        }
    }

    public void PlaySound(int channelKey)
    {
        FMOD.RESULT result = FMODUnity.RuntimeManager.CoreSystem.playSound(trackSounds[channelKey], trackSoundChannelGroup, false, out trackSoundChannels[channelKey]);
    
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Failed to play sound: " + result);
            return;
        }

        trackSoundChannels[channelKey].isPlaying(out bool isPlaying);
        if (isPlaying)
        {
            trackSoundChannels[channelKey].stop();
        }

        trackSoundChannels[channelKey].setPaused(false);
    }

    void OnDestroy() {
        foreach (var sound in trackSounds) {
            sound.release();
        }
        trackSounds = null;
    }
}
