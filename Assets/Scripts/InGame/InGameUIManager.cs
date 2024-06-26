using B83.Image.BMP;
using BMS;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    // Dict�� ���� �� �ð��� ���� BGA ����
    // Dict�� Ű�� ������� Index ���� �� Index�� ������ Dict�� �� ����
    // Start���� ��Ÿ�ӿ��� �ؽ��� �ε�ó��
    // mp4�� ���� �������� / �̹��� ���Ͽ� ���� ����ó�� �ʼ�
    public static InGameUIManager Instance { get; private set; }

    private string[] videoExtensions = { ".mp4" };
    private Dictionary<int, Texture2D> bgaImages = new();
    [SerializeField]
    private UnityEngine.UI.RawImage bgaImageFrame;
    [SerializeField]
    private TextMeshProUGUI debugText;

    Queue<Action> jobs = new Queue<Action>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void LoadBGAAssets(Dictionary<int, string> bgaMap, BGASequence.BGAFlagState flag)
    {
        if (flag == BGASequence.BGAFlagState.Image)
        {
            foreach (var bgaKey in bgaMap.Keys)
            {
                string filePath = bgaMap[bgaKey];
                string fileExtension = Path.GetExtension(filePath).ToLower();

                if (!File.Exists(filePath))
                {
                    continue;
                }

                if (fileExtension == ".bmp")
                {
                    BMPLoader loader = new BMPLoader();
                    BMPImage img = loader.LoadBMP(filePath);
                    Texture2D texture = img.ToTexture2D();
                    bgaImages[bgaKey] = texture;
                    continue;
                }
                else if (fileExtension == ".jpg" || fileExtension == ".png")
                {
                    byte[] fileData = File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(1, 1);

                    if (texture.LoadImage(fileData))
                    {
                        bgaImages[bgaKey] = texture;
                    }
                    continue;
                }
            }
        }
        else
        {
            
        }
    }

    public void UpdateBGA(int bgaKey, BGASequence.BGAFlagState flag)
    {
        if (flag == BGASequence.BGAFlagState.Image)
        {
            jobs.Enqueue(() => 
                {
                    bgaImageFrame.texture = bgaImages[bgaKey];
                    debugText.text = $"Current BGA Key: {bgaKey}";
                }
            );
        }
        else
        {

        }
    }

    private void Update()
    {
        while (jobs.Count > 0)
            jobs.Dequeue().Invoke();
    }
}
