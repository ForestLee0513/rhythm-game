using B83.Image.BMP;
using BMS;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class InGameUIManager : MonoBehaviour
{
    // Dict로 저장 후 시간에 따라 BGA 변경
    // Dict의 키를 기반으로 Index 생성 후 Index의 값으로 Dict의 값 참조
    // Start에서 런타임에서 텍스쳐 로드처리
    // mp4와 같은 영상파일 / 이미지 파일에 대한 예외처리 필수
    public static InGameUIManager Instance { get; private set; }

    // 비디오 기반 BGA
    private Dictionary<int, string> videoBGAPathMap = new();
    [SerializeField]
    private VLCPlayerExample videoBGAPlayer;

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
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            videoBGAPlayer = gameObject.GetComponent<VLCPlayerExample>();
        }
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
                    byte[] fileData = File.ReadAllBytes(filePath);
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
                videoBGAPathMap.Add(bgaKey, filePath);
            }
        }
    }

    public void UpdateVideoBGA(int bgaKey)
    {
        videoBGAPlayer.Open(videoBGAPathMap[bgaKey]);
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
            UnityEngine.Debug.LogError("비디오 형식은 UpdateVideoBGA() 매서드를 사용해주세요.");
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
            UnityEngine.Debug.LogError("비디오는 레이어 BGA를 지원하지 않아요.");
        }
    }

    private void ToggleGameObject<T>(T targetGameObject) where T : MonoBehaviour
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
    }

    private void OnDestroy()
    {
        baseBGAChangeJobs.Clear();
        layerBGAChangeJobs.Clear();
        bpmChangeJobs.Clear();
    }
}
