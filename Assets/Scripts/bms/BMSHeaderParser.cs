using System;
using System.IO;
using UnityEngine;

public class BMSHeaderParser : ChartDecoder
{
    #region Random Statements
    bool isRandom = false;
    bool isIfStatementTrue = false;
    bool isCheckIfstatementStarted = false;
    int randomResult = 0;
    #endregion

    public BMSHeaderParser(string path) : base(path)
    {
        parseData += ParseHeader;
        ReadFile();
    }

    private void ParseHeader(string line)
    {
        string headerKey = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(0, line.IndexOf(" ")) : line;
        string headerValue = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(" ") + 1) : "";

        if (headerKey == "#RANDOM")
        {
            isRandom = true;
            Int32.TryParse(headerValue, out int randomNumber);
            randomResult = new System.Random().Next(1, randomNumber + 1);
        }

        if (headerKey == "#ENDRANDOM")
        {
            isRandom = false;
            randomResult = 0;
        }

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

        if (headerKey == "#ENDIF")
        {
            isCheckIfstatementStarted = false;
            isIfStatementTrue = false;
        }

        if (isRandom == false || (isIfStatementTrue == true && isCheckIfstatementStarted == true) || isCheckIfstatementStarted == false)
        {
            switch (headerKey)
            {
                case "#PLAYER":
                    Int32.TryParse(headerValue, out TrackInfo.playerType);
                    break;
                case "#GENRE":
                    TrackInfo.genre = headerValue;
                    break;
                case "#TITLE":
                    TrackInfo.title = headerValue;
                    break;
                case "#ARTIST":
                    TrackInfo.artist = headerValue;
                    break;
                case "#PLAYLEVEL":
                    Int32.TryParse(headerValue, out TrackInfo.playLevel);
                    break;
                case "#RANK":
                    Int32.TryParse(headerValue, out TrackInfo.rank);
                    break;
                case "#TOTAL":
                    Single.TryParse(headerValue, out TrackInfo.total);
                    break;
                case "#STAGEFILE":
                    TrackInfo.stageFile = Path.Combine(Directory.GetParent(path).FullName, headerValue);
                    break;
                case "#LNOBJ":
                    TrackInfo.lnobjList.Add(headerValue);
                    break;
            }

            if (headerKey.StartsWith("#WAV"))
            {
                TrackInfo.audioFileNames.Add(Base36.Decode36(headerKey.Substring(4)), Path.Combine(Directory.GetParent(path).FullName, System.Web.HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(headerValue))));
            }
            if (headerKey.StartsWith("#BMP"))
            {
                TrackInfo.imageFileNames.Add(headerKey.Substring(4), Path.Combine(Directory.GetParent(path).FullName, System.Web.HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(headerValue))));
            }

            // STOP //
            if (headerKey.StartsWith("#STOP"))
            {
                Int32.TryParse(headerValue, out int stop);
                TrackInfo.stopTable.Add(headerKey.Substring(5), stop);
            }

            // BPM //
            if (line.StartsWith("#BPM"))
            {
                if (line[4] == ' ')
                {
                    // �Ϲ� BPM
                    float.TryParse(headerValue, out TrackInfo.bpm);
                }
                else
                {
                    // ���� BPM
                    string[] bpmKeyValuePair = line.Substring(4).Split(" ");
                    string bpmKey = bpmKeyValuePair[0];
                    Single.TryParse(bpmKeyValuePair[1], out float bpmValue);

                    TrackInfo.bpmTable.Add(bpmKey, bpmValue);
                }
            }
        }
    }
}