using B83.Image.BMP;
using BMS;
using LibVLCSharp;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    // Dict로 저장 후 시간에 따라 BGA 변경
    // Dict의 키를 기반으로 Index 생성 후 Index의 값으로 Dict의 값 참조
    // Start에서 런타임에서 텍스쳐 로드처리
    // mp4와 같은 영상파일 / 이미지 파일에 대한 예외처리 필수
    public static InGameUIManager Instance { get; private set; }

    // 비디오 기반 BGA
    private string[] videoExtensions = { ".mp4" };
    public static LibVLC libVLC;
    private MediaPlayer mediaPlayer;
    private Dictionary<int, string> videoBGAPath = new();
    [SerializeField]
    private RawImage videoBGACanvas;
    Texture2D _vlcTexture = null; //This is the texture libVLC writes to directly. It's private.
    public RenderTexture texture = null; //We copy it into this texture which we actually use in unity.
    public bool flipTextureX = true;
    public bool flipTextureY = true;
    public bool logToConsole = false;
    private AspectRatioFitter videoBGAAspectRatioFitter;

    // 이미지 기반 BGA
    private Dictionary<int, Texture2D> bgaImages = new();
    private Dictionary<int, Texture2D> layerBGAImages = new();
    [SerializeField]
    private RawImage baseBGACanvas;
    [SerializeField]
    private RawImage layerBGACanvas;

    // 디버깅용 텍스트
    [SerializeField]
    private TextMeshProUGUI debugBGAText;
    [SerializeField]
    private TextMeshProUGUI debugBPMText;

    // 메인 스레드 작업 큐
    Queue<Action> baseBGAChangeJobs = new Queue<Action>();
    Queue<Action> layerBGAChangeJobs = new Queue<Action>();
    Queue<Action> bpmChangeJobs = new Queue<Action>();
    Queue<Action> videoBGAChangeJobs = new Queue<Action>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreateLibVLC();
            CreateMediaPlayer();
        }
    }

    void CreateLibVLC()
    {
        Debug.Log("CreateLibVLC");
        //Dispose of the old libVLC if necessary
        if (libVLC != null)
        {
            libVLC.Dispose();
            libVLC = null;
        }

        Core.Initialize(UnityEngine.Application.dataPath); //Load VLC dlls
        libVLC = new LibVLC(enableDebugLogs: true); //You can customize LibVLC with advanced CLI options here https://wiki.videolan.org/VLC_command-line_help/

        //Setup Error Logging
        UnityEngine.Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        libVLC.Log += (s, e) =>
        {
            //Always use try/catch in LibVLC events.
            //LibVLC can freeze Unity if an exception goes unhandled inside an event handler.
            try
            {
                if (logToConsole)
                {
                    Debug.Log(e.FormattedLog);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception caught in libVLC.Log: \n" + ex.ToString());
            }
        };
    }

    void CreateMediaPlayer()
    {
        Debug.Log(" CreateMediaPlayer");
        if (mediaPlayer != null)
        {
            DestroyMediaPlayer();
        }
        mediaPlayer = new MediaPlayer(libVLC);
    }

    void DestroyMediaPlayer()
    {
        Debug.Log("DestroyMediaPlayer");
        mediaPlayer?.Stop();
        mediaPlayer?.Dispose();
        mediaPlayer = null;
    }

    // BPM //
    public void UpdateBPMText(double bpm)
    {
        bpmChangeJobs.Enqueue(() => debugBPMText.text = $"BPM: {bpm}");
    }

    // BGA // 
    public void LoadBGAAssets(Dictionary<int, string> bgaMap, BGASequence.BGAFlagState flag)
    {
        if (flag == BGASequence.BGAFlagState.Image)
        {
            foreach (var bgaKey in bgaMap.Keys)
            {
                string filePath = bgaMap[bgaKey];
                string fileExtension = Path.GetExtension(filePath).ToLower();

                if (!System.IO.File.Exists(filePath))
                {
                    continue;
                }

                if (fileExtension == ".bmp")
                {
                    BMPLoader loader = new BMPLoader();
                    BMPImage img = loader.LoadBMP(filePath);
                    Texture2D texture = img.ToTexture2D();
                    bgaImages[bgaKey] = texture;
                    layerBGAImages[bgaKey] = ApplyChromaKey(texture, Color.black);
                    continue;
                }
                else if (fileExtension == ".jpg" || fileExtension == ".png")
                {
                    byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(1, 1);

                    if (texture.LoadImage(fileData))
                    {
                        bgaImages[bgaKey] = texture;
                        layerBGAImages[bgaKey] = ApplyChromaKey(texture, Color.black);
                    }
                    continue;
                }
            }
        }
        else
        {
            foreach (var bgaKey in bgaMap.Keys)
            {
                string filePath = bgaMap[bgaKey].Trim(new char[] { '"' });
                videoBGAPath.Add(bgaKey, filePath);
            }
        }
    }

    public void UpdateVideoBGA(int bgaKey)
    {
        string path = videoBGAPath[bgaKey].Trim(new char[] { '"' });

        videoBGAChangeJobs.Enqueue(() =>
        {
            if (videoBGACanvas.gameObject.activeSelf == false)
            {
                videoBGACanvas.gameObject.SetActive(true);
            }

            if (mediaPlayer.Media != null)
            {
                mediaPlayer.Media.Dispose();
            }

            mediaPlayer.Media = new Media(new Uri(path));
            mediaPlayer.Play();
        });
    }

    public void UpdateBaseBGA(int bgaKey, BGASequence.BGAFlagState flag)
    {
        if (flag == BGASequence.BGAFlagState.Image)
        {
            baseBGAChangeJobs.Enqueue(() =>
            {
                if (baseBGACanvas.gameObject.activeSelf == false)
                {
                    ToggleGameObject(baseBGACanvas);
                }

                Texture texture = bgaImages[bgaKey];
                baseBGACanvas.texture = texture;
                debugBGAText.text = $"Current BGA Key: {bgaKey}";

                // 컴포넌트 참조하는 코드가 Update에 있는지 검증 필요.
                // 있다면 변경예정.
                AspectRatioFitter baseBGAAspectRatioFitter = baseBGACanvas.GetComponent<AspectRatioFitter>();
                float aspectRatio = texture.width / texture.height;
                baseBGAAspectRatioFitter.aspectRatio = aspectRatio > 0 ? aspectRatio : 1;
            });
        }
        else
        {
            Debug.LogError("비디오 형식은 UpdateVideoBGA() 매서드를 사용해주세요.");
        }
    }

    public void UpdateLayerBGA(int bgaKey, BGASequence.BGAFlagState flag)
    {
        if (flag == BGASequence.BGAFlagState.Image)
        {
            layerBGAChangeJobs.Enqueue(() =>
            {
                if (layerBGACanvas.gameObject.activeSelf == false)
                {
                    ToggleGameObject(layerBGACanvas);
                }

                Texture texture = layerBGAImages[bgaKey];
                layerBGACanvas.texture = texture;
                AspectRatioFitter layerBGAAspectRatioFitter = layerBGACanvas.GetComponent<AspectRatioFitter>();
                float aspectRatio = texture.width / texture.height;
                layerBGAAspectRatioFitter.aspectRatio = aspectRatio > 0 ? aspectRatio : 1;
            });
        }
        else
        {
            Debug.LogError("비디오는 레이어 BGA를 지원하지 않아요.");
        }
    }

    private void ToggleGameObject<T>(T targetGameObject) where T: MonoBehaviour
    {
        targetGameObject.gameObject.SetActive(!targetGameObject.gameObject.activeSelf);
    }

    private Texture2D ApplyChromaKey(Texture2D originalTexture, Color chromaKeyColor)
    {
        Texture2D newTexture = new Texture2D(originalTexture.width, originalTexture.height);
        newTexture.SetPixels(originalTexture.GetPixels());
        Color[] pixels = newTexture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r == chromaKeyColor.r &&
                pixels[i].g == chromaKeyColor.g &&
                pixels[i].b == chromaKeyColor.b)
            {
                pixels[i].a = 0; // 알파 값을 0으로 설정하여 투명하게 만듭니다
            }
        }

        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return newTexture;
    }

    private void Update()
    {
        while (baseBGAChangeJobs.Count > 0)
            baseBGAChangeJobs.Dequeue().Invoke();

        while (layerBGAChangeJobs.Count > 0)
            layerBGAChangeJobs.Dequeue().Invoke();

        while (bpmChangeJobs.Count > 0)
            bpmChangeJobs.Dequeue().Invoke();

        while (videoBGAChangeJobs.Count > 0)
            videoBGAChangeJobs.Dequeue().Invoke();
    }

    public void UpdateVideoCanvas()
    {
        if (mediaPlayer.IsPlaying == false || mediaPlayer.Media == null || mediaPlayer == null)
        {
            return;
        }

        if (videoBGAAspectRatioFitter == null)
        {
            videoBGAAspectRatioFitter = videoBGACanvas.GetComponent<AspectRatioFitter>();
        }

        uint height = 0;
        uint width = 0;
        mediaPlayer.Size(0, ref width, ref height);

        //Automatically resize output textures if size changes
        if (_vlcTexture == null || _vlcTexture.width != width || _vlcTexture.height != height)
        {
            ResizeOutputTextures(width, height);
        }

        if (_vlcTexture != null)
        {
            //Update the vlc texture (tex)
            var texptr = mediaPlayer.GetTexture(width, height, out bool updated);
            if (updated)
            {
                _vlcTexture.UpdateExternalTexture(texptr);

                //Copy the vlc texture into the output texture, automatically flipped over
                var flip = new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
                Graphics.Blit(_vlcTexture, texture, flip, Vector2.zero); //If you wanted to do post processing outside of VLC you could use a shader here.
                videoBGAAspectRatioFitter.aspectRatio = _vlcTexture.width / _vlcTexture.height;
            }
        }
    }

    void ResizeOutputTextures(uint px, uint py)
    {
        var texptr = mediaPlayer.GetTexture(px, py, out bool updated);
        if (px != 0 && py != 0 && updated && texptr != IntPtr.Zero)
        {
            //If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
            if (GetVideoOrientation() == VideoOrientation.BottomRight)
            {
                uint swap = px;
                px = py;
                py = swap;
            }

            _vlcTexture = Texture2D.CreateExternalTexture((int)px, (int)py, TextureFormat.RGBA32, false, true, texptr); //Make a texture of the proper size for the video to output to
            texture = new RenderTexture(_vlcTexture.width, _vlcTexture.height, 0, RenderTextureFormat.ARGB32); //Make a renderTexture the same size as vlctex

            videoBGACanvas.texture = texture;
        }
    }

    public VideoOrientation? GetVideoOrientation()
    {
        var tracks = mediaPlayer?.Tracks(TrackType.Video);

        if (tracks == null || tracks.Count == 0)
            return null;

        var orientation = tracks[0]?.Data.Video.Orientation; //At the moment we're assuming the track we're playing is the first track

        return orientation;
    }

    private void OnDestroy()
    {
        baseBGAChangeJobs.Clear();
        layerBGAChangeJobs.Clear();
        bpmChangeJobs.Clear();
        videoBGAChangeJobs.Clear();

        DestroyMediaPlayer();
    }
}
