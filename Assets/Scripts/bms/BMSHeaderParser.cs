using System;
using System.IO;
using UnityEngine;

namespace BMS
{
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
                    case "#SUBARTIST":
                        TrackInfo.subArtist = headerValue;
                        break;
                    case "#PLAYLEVEL":
                        Int32.TryParse(headerValue, out TrackInfo.playLevel);
                        break;
                    case "#RANK":
                        Int32.TryParse(headerValue, out TrackInfo.rank);
                        break;
                    case "#TOTAL":
                        Double.TryParse(headerValue, out TrackInfo.total);
                        break;
                    case "#STAGEFILE":
                        TrackInfo.stageFile = Path.Combine(Directory.GetParent(path).FullName, headerValue);
                        break;
                    case "#LNOBJ":
                        TrackInfo.lnobj = Decode36(headerValue);
                        break;
                    case "#LNTYPE":
                        Int32.TryParse(headerValue, out int lnType);
                        TrackInfo.lnType = lnType;
                        break;
                    case "#SUBTITLE":
                        TrackInfo.subTitle = headerValue;
                        break;
                }

                if (headerKey.StartsWith("#WAV"))
                {
                    TrackInfo.audioFileNames.Add(Decode36(headerKey.Substring(4)), Path.Combine(Directory.GetParent(path).FullName, Path.GetFileNameWithoutExtension(headerValue)));
                }
                if (headerKey.StartsWith("#BMP"))
                {
                    TrackInfo.imageFileNames.Add(Decode36(headerKey.Substring(4)), Path.Combine(Directory.GetParent(path).FullName, headerValue));
                }

                // STOP //
                if (headerKey.StartsWith("#STOP"))
                {
                    Int32.TryParse(headerValue, out int stop);
                    TrackInfo.stopTable.Add(Decode36(headerKey.Substring(5)), stop / 192);
                }

                // BPM //
                if (line.StartsWith("#BPM"))
                {
                    if (line[4] == ' ')
                    {
                        double.TryParse(headerValue, out TrackInfo.bpm);
                    }
                    else
                    {
                        string[] bpmKeyValuePair = line.Substring(4).Split(" ");
                        string bpmKey = bpmKeyValuePair[0];
                        Double.TryParse(bpmKeyValuePair[1], out double bpmValue);

                        TrackInfo.bpmTable.Add(Decode36(bpmKey), bpmValue);
                    }
                }
            }
        }
    }
}