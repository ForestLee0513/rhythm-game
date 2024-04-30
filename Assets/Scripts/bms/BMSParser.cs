using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class BMSParser
{
    #region Random Statements
    public TrackInfo TrackInfo { get { return trackInfo; } }
    bool isRandom = false;
    bool isIfStatementTrue = false;
    bool isCheckIfstatementStarted = false;
    int randomResult = 0;
    #endregion
    #region Track File info
    TrackInfo trackInfo = new TrackInfo();
    string path = "";
    #endregion

    #region Initializer
    public BMSParser(string path)
    {
        this.path = path;
    }
    #endregion

    // ��� //
    // ��� ���� �ҷ�����
    public void ReadHeader()
    {
        Regex mainDataSyntaxRegex = new Regex("#\\d{5}");
        using (var reader = new StreamReader(path, Encoding.GetEncoding(932)))
        {
            do
            {
                string line = reader.ReadLine();
                if (mainDataSyntaxRegex.IsMatch(line))
                {
                    break;
                }

                ParseHeader(line);
            } while (!reader.EndOfStream);
        }
    }

    // ��� ���� �Ľ�
    private void ParseHeader(string line)
    {
        string headerKey = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(0, line.IndexOf(" ")) : line;
        string headerValue = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(" ") + 1) : "";

        // Random�� ���� ��ó�� (#RANDOM ���� / #ENDRANDOM���� ����) //
        // ���� �� ����
        if (headerKey == "#RANDOM")
        {
            isRandom = true;
            Int32.TryParse(headerValue, out int randomNumber);
            randomResult = new System.Random().Next(1, randomNumber + 1);
        }

        // ���� Ż��
        if (headerKey == "#ENDRANDOM")
        {
            isRandom = false;
            randomResult = 0;
        }

        // ���ǿ� ���� ��ó�� (#IF ���� / #ENDIF�� ����) // 
        // ���� �˻�
        if (headerKey == "#IF" && Int32.TryParse(headerValue, out int parsedHeaderValueNumber) && isRandom == true)
        {
            isCheckIfstatementStarted = true;
            if (parsedHeaderValueNumber == randomResult)
            {
                isIfStatementTrue = true;
            }
            else
            {
                isIfStatementTrue = false;
            }
        }

        // ���� Ż��
        if (headerKey == "#ENDIF")
        {
            isCheckIfstatementStarted = false;
            isIfStatementTrue = false;
        }

        // Ʈ�� ���� ����
        if (isRandom == false || (isIfStatementTrue == true && isCheckIfstatementStarted == true) || isCheckIfstatementStarted == false)
        {
            switch (headerKey)
            {
                case "#PLAYER":
                    Int32.TryParse(headerValue, out trackInfo.playerType);
                    break;
                case "#GENRE":
                    trackInfo.genre = headerValue;
                    break;
                case "#TITLE":
                    trackInfo.title = headerValue;
                    break;
                case "#ARTIST":
                    trackInfo.artist = headerValue;
                    break;
                case "#PLAYLEVEL":
                    Int32.TryParse(headerValue, out trackInfo.playLevel);
                    break;
                case "#RANK":
                    Int32.TryParse(headerValue, out trackInfo.rank);
                    break;
                case "#TOTAL":
                    Single.TryParse(headerValue, out trackInfo.total);
                    break;
                case "#STAGEFILE":
                    trackInfo.stageFile = Path.Combine(Directory.GetParent(path).FullName, headerValue);
                    break;
            }

            // ���� ���� ���� �Ľ� //
            // �����
            if (headerKey.StartsWith("#WAV"))
            {
                trackInfo.audioFileNames.Add(headerKey.Substring(4), Path.Combine(Directory.GetParent(path).FullName, System.Web.HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(headerValue))));
            }
            // BGA �̹��� (mp4�� �������� ����.)
            if (headerKey.StartsWith("BMP"))
            {
                trackInfo.imageFileNames.Add(headerKey.Substring(4), Path.Combine(Directory.GetParent(path).FullName, System.Web.HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(headerValue))));
            }

            // BPM //
            if (line.StartsWith("#BPM"))
            {
                if (line[4] == ' ')
                {
                    // �Ϲ� BPM
                    float.TryParse(headerValue, out trackInfo.bpm);
                }
                else
                {
                    // ���� BPM
                    string[] bpmKeyValuePair = line.Substring(4).Split(" ");
                    string bpmKey = bpmKeyValuePair[0];
                    Single.TryParse(bpmKeyValuePair[1], out float bpmValue);

                    trackInfo.bpmTable.Add(bpmKey, bpmValue);
                }
            }
        }
    }

    // ���� ä�� ������ //
    // BMS ���� �ҷ�����
    public void ParseMainData()
    {
        using (var reader = new StreamReader(path, Encoding.GetEncoding(932)))
        {
            do
            {
                string line = reader.ReadLine();
                ReadMainData(line);
            } while (!reader.EndOfStream);
        }
    }

    // ���� ä�� �Ľ�
    private void ReadMainData(string line)
    {

    }
}