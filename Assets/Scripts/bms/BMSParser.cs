using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class BMSParser
{
    #region Random Statements
    bool isRandom = false;
    bool isIfStatementTrue = false;
    bool isCheckIfstatementStarted = false;
    int randomResult = 0;
    #endregion
    #region Track File info
    string path = "";
    public TrackInfo TrackInfo { get { return trackInfo; } }
    TrackInfo trackInfo = new TrackInfo();
    #endregion

    #region Initializer
    public BMSParser(string path)
    {
        this.path = path;
        trackInfo.path = path;
    }

    public BMSParser(TrackInfo trackInfo)
    {
        this.trackInfo = trackInfo;
        path = trackInfo.path;
    }
    #endregion


    private void ResetRandomState()
    {
        isRandom = false;
        isIfStatementTrue = false;
        isCheckIfstatementStarted = false;
        randomResult = 0;
    }

    // ��� //
    // ��� ���� �ҷ�����
    public void ReadHeader()
    {
        using (var reader = new StreamReader(path, Encoding.GetEncoding(932)))
        {
            do
            {
                string line = reader.ReadLine();

                ParseHeader(line);
            } while (!reader.EndOfStream);
        }
        ResetRandomState();
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
                case "#LNOBJ":
                    trackInfo.lnobjList.Add(headerValue);
                    break;
            }

            // ���� ���� ���� �Ľ� //
            // �����
            if (headerKey.StartsWith("#WAV"))
            {
                trackInfo.audioFileNames.Add(Base36.Decode36(headerKey.Substring(4)), Path.Combine(Directory.GetParent(path).FullName, System.Web.HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(headerValue))));
            }
            // BGA �̹��� (mp4�� �������� ����.)
            if (headerKey.StartsWith("#BMP"))
            {
                trackInfo.imageFileNames.Add(headerKey.Substring(4), Path.Combine(Directory.GetParent(path).FullName, System.Web.HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(headerValue))));
            }

            // STOP //
            if (headerKey.StartsWith("#STOP"))
            {
                Int32.TryParse(headerValue, out int stop);
                trackInfo.stopTable.Add(headerKey.Substring(5), stop);
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
        bool isMainDataParseStarted = false;
        using (var reader = new StreamReader(path, Encoding.GetEncoding(932)))
        {
            do
            {
                string line = reader.ReadLine();
                if (isMainDataParseStarted)
                {
                    ReadMainData(line);
                }
                else
                {
                    isMainDataParseStarted = line.IndexOf(":") > -1;
                }
            } while (!reader.EndOfStream);
        }

        ResetRandomState();
    }

    // ���� ä�� �Ľ�
    private void ReadMainData(string line)
    {
        string statementDataKey = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(0, line.IndexOf(" ")) : line;
        string statementDataValue = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(" ") + 1) : "";

        string mainDataKey = line.IndexOf(":") > -1 && line.StartsWith("#") ? line.Substring(1, line.IndexOf(":") - 1) : "";
        string mainDataValue = line.IndexOf(":") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(":") + 1) : "";

        // Random�� ���� ��ó�� (#RANDOM ���� / #ENDRANDOM���� ����) //
        // ���� �� ����
        if (statementDataKey == "#RANDOM")
        {
            isRandom = true;
            Int32.TryParse(statementDataValue, out int randomNumber);
            randomResult = new System.Random().Next(1, randomNumber + 1);
        }

        // ���� Ż��
        if (statementDataKey == "#ENDRANDOM")
        {
            isRandom = false;
            randomResult = 0;
        }

        // ���ǿ� ���� ��ó�� (#IF ���� / #ENDIF�� ����) // 
        // ���� �˻�
        if (statementDataKey == "#IF" && Int32.TryParse(statementDataValue, out int parsedStatementDataValue) && isRandom == true)
        {
            isCheckIfstatementStarted = true;
            if (parsedStatementDataValue == randomResult)
            {
                isIfStatementTrue = true;
            }
            else
            {
                isIfStatementTrue = false;
            }
        }

        // ���� Ż��
        if (statementDataKey == "#ENDIF")
        {
            isCheckIfstatementStarted = false;
            isIfStatementTrue = false;
        }

        if (isRandom == false || (isIfStatementTrue == true && isCheckIfstatementStarted == true) || isCheckIfstatementStarted == false)
        {
            if (mainDataKey != "" && mainDataValue != "")
            {
                // ����
                Debug.Log(mainDataKey.Substring(0,3));
            }
        }
    }
}