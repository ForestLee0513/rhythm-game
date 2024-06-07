using System.IO;
using BMS;
using Unity.VisualScripting;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public static Metronome Instance { get; private set; }
    public double Bpm { get; private set; }
    public double BeatInterval { get; private set; }
    private double beatPerBar;
    public double NormalizedTime { get; private set; }
    public int BarCount { get; private set; }
    public double Beat { get; private set; }
    private double lastBeat;
    private double nextBeatTime;
    private double beatStartTime;
    // private double lastBpm;
    private int totalBarCount;
    private double lastBpm;


    // 테스트 메트로놈 사운드
    private FMOD.ChannelGroup tickSoundChannelGroup;
    private FMOD.Sound[] tickSounds;
    private FMOD.Channel[] tickSoundChannels;

    public void Init(double bpm, int totalBarCount, double beat)
    {
        Bpm = bpm;
        this.totalBarCount = totalBarCount;
        Beat = beat;
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        BeatInterval = Beat * 60_000.0f / Bpm;
        beatStartTime = Time.time * 1000;
        nextBeatTime = beatStartTime + BeatInterval;
        beatPerBar = Beat * 4;

        // 테스트용 메트로놈 Tick SFX 로드 처리
        // Assets/Audio/SFX/Metronome/tick.mp3 파일이 존재하는 경우에만 로드
        if (File.Exists(Path.Combine(Application.dataPath, "Audio", "SFX", "Metronome", "tick.mp3")))
        {
            FMODUnity.RuntimeManager.CoreSystem.createChannelGroup("TickSoundGroup", out tickSoundChannelGroup);
            tickSoundChannels = new FMOD.Channel[1];
            tickSounds = new FMOD.Sound[1];

            FMODUnity.RuntimeManager.CoreSystem.createSound(Path.Combine(Application.dataPath, "Audio", "SFX", "Metronome", "tick.mp3"), FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out tickSounds[0]);
            tickSoundChannels[0].setChannelGroup(tickSoundChannelGroup);
        }
    }

    void Update()
    {
        UpdateMetronomeTick();
    }

    private void UpdateMetronomeTick()
    {
        if (BarCount == totalBarCount)
        {
            return;
        }

        double currentTime = Time.time * 1000;

        if (Bpm != lastBpm || Beat != lastBeat)
        {
            BeatInterval = Beat * 60_000.0f / Bpm;
            beatStartTime = Time.time * 1000;
            nextBeatTime = beatStartTime + BeatInterval;
            beatPerBar = Beat * 4;
            lastBpm = Bpm;
            lastBeat = Beat;
        }

        if (currentTime >= nextBeatTime)
        {
            PlaySound(0);
            beatStartTime = nextBeatTime;
            nextBeatTime += BeatInterval;
        }
        
        BarCount = (int)(currentTime / (BeatInterval * beatPerBar)) + 1;
        NormalizedTime = currentTime - beatStartTime;
        
        Debug.Log($"현재 마디는 {BarCount} 마디 별 소요 시간은 {NormalizedTime} 입니다. 현재 비트 간격은 {beatPerBar} 총 시간은 {currentTime}");
    }

    public void SetBpm(double newBpm)
    {
        Bpm = newBpm;
    }

    public void SetBeat(double beat)
    {
        Beat = beat;
    }


    public void PlaySound(int channelKey)
    {
        FMOD.RESULT result = FMODUnity.RuntimeManager.CoreSystem.playSound(tickSounds[channelKey], tickSoundChannelGroup, false, out tickSoundChannels[channelKey]);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Failed to play sound: " + result);
            return;
        }

        tickSoundChannels[channelKey].getPaused(out bool isPaused);
        if (isPaused)
        {
            tickSoundChannels[channelKey].setPaused(false);
        }
        else
        {
            tickSoundChannels[channelKey].stop();
            FMODUnity.RuntimeManager.CoreSystem.playSound(tickSounds[channelKey], tickSoundChannelGroup, false, out tickSoundChannels[channelKey]);
        }
    }


    void OnDestroy()
    {
        foreach (var sound in tickSounds) {
            sound.release();
        }
        tickSounds = null;
    }
}