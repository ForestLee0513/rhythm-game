using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BMS
{
    public class BMSHeaderParser : ChartDecoder
    {
        public BMSHeaderParser(string path) : base(path)
        {
            parseData += ParseHeader;
            ReadFile();
        }

        private void ParseHeader(string line)
        {
            string headerKey = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(0, line.IndexOf(" ")) : line;
            string headerValue = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(" ") + 1) : "";

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
                    string fileName = string.Concat(headerValue.Split(Path.GetInvalidPathChars()));
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    TrackInfo.stageFile = Path.Combine(Directory.GetParent(path).FullName, fileNameWithoutExtension);
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
                string fileName = string.Concat(headerValue.Split(Path.GetInvalidPathChars()));
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                TrackInfo.audioFileNames.Add(Decode36(headerKey.Substring(4)), Path.Combine(Directory.GetParent(path).FullName, fileNameWithoutExtension));
            }
            if (headerKey.StartsWith("#BMP"))
            {
                string fileName = string.Concat(headerValue.Split(Path.GetInvalidPathChars()));
                string fileExtension = Path.GetExtension(headerValue).ToLower();

                if (fileExtension == ".mpg" || fileExtension == ".mpeg")
                {
                    TrackInfo.videoFileNames.Add(Decode36(headerKey.Substring(4)), Path.Combine(Directory.GetParent(path).FullName, fileName));
                }
                else
                {
                    TrackInfo.imageFileNames.Add(Decode36(headerKey.Substring(4)), Path.Combine(Directory.GetParent(path).FullName, fileName));
                }
            }

            // STOP //
            if (headerKey.StartsWith("#STOP"))
            {
                Double.TryParse(headerValue, out double stop);
                TrackInfo.stopTable.Add(Decode36(headerKey.Substring(5)), stop / 192.0);
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
