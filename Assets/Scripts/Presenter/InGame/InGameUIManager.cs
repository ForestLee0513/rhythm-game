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
    // Dict�� ���� �� �ð��� ���� BGA ����
    // Dict�� Ű�� ������� Index ���� �� Index�� ������ Dict�� �� ����
    // Start���� ��Ÿ�ӿ��� �ؽ��� �ε�ó��
    // mp4�� ���� �������� / �̹��� ���Ͽ� ���� ����ó�� �ʼ�
    public static InGameUIManager Instance { get; private set; }

    // ���� ��� BGA
    private Dictionary<int, string> videoBGAPathMap = new();
    [SerializeField]
    private VLCPlayerExample videoBGAPlayer;

    // �̹��� ��� BGA
    private Dictionary<int, Texture2D> bgaImages = new();
    private Dictionary<int, Texture2D> layerBGAImages = new();
    [SerializeField]
    private RawImage baseBGACanvas;
    [SerializeField]
    private RawImage layerBGACanvas;

    // ������ �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI debugBGAText;
    [SerializeField]
    private TextMeshProUGUI debugBPMText;

    // ���� ������ �۾� ť
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

                // ������Ʈ �����ϴ� �ڵ尡 Update�� �ִ��� ���� �ʿ�.
                // �ִٸ� ���濹��.
                AspectRatioFitter baseBGAAspectRatioFitter = baseBGACanvas.GetComponent<AspectRatioFitter>();
                float aspectRatio = texture.width / texture.height;
                baseBGAAspectRatioFitter.aspectRatio = aspectRatio > 0 ? aspectRatio : 1;
            });
        }
        else
        {
            UnityEngine.Debug.LogError("���� ������ UpdateVideoBGA() �ż��带 ������ּ���.");
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
            UnityEngine.Debug.LogError("������ ���̾� BGA�� �������� �ʾƿ�.");
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
                pixels[i].a = 0; // ���� ���� 0���� �����Ͽ� �����ϰ� ����ϴ�
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
