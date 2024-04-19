using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BMSFileSystem : MonoBehaviour
{
    private string rootPath;
    private static List<TrackInfo> trackInfoList = new List<TrackInfo>();
    public static List<TrackInfo> TrackInfoList { get { return trackInfoList; } }

    private void Awake()
    {
        ImportFiles();
    }

    private void ImportFiles()
    {
        // path 없을 때 새로운 path 지정
        // 추후 배열로 대응 예정
        if (rootPath == "" || rootPath == null)
        {
#if UNITY_EDITOR
            rootPath = @"c:\bmsFiles";
#elif UNITY_STANDALONE
            rootPath = @$"{Application.dataPath}/bmsFiles";
#endif
        }

        // load all bms files..
        List<string> extensions = new List<string> { "bms", "bme", "bml", "pms" };
        IEnumerable<string> bmsFilePaths = Directory
            .EnumerateFiles(rootPath, "*.*", SearchOption.AllDirectories)
            .Where(s => extensions.Contains(Path.GetExtension(s).TrimStart(".").ToLowerInvariant()));

        foreach (string bmsFilePath in bmsFilePaths)
        {
            ParseSongInfoHeader(bmsFilePath);
        }
    }

    private void ParseSongInfoHeader(string path)
    {
        TrackInfo trackInfo = new TrackInfo();
        bool isRandom = false;
        bool isIfStatementTrue = false;
        bool isCheckIfstatementStarted = false;
        int randomResult = 0;

        trackInfo.path = path;
        using (var reader = new StreamReader(path))
        {
            do
            {
                string line = reader.ReadLine();
                string headerKey = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(0, line.IndexOf(" ")) : line;
                string headerValue = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(" ") + 1) : "";

                // Random에 대한 전처리 (#RANDOM 숫자 / #ENDRANDOM으로 구분) //
                // 랜덤 값 지정
                if (headerKey == "#RANDOM")
                {
                    isRandom = true;
                    Int32.TryParse(headerValue, out int randomNumber);
                    randomResult = new System.Random().Next(1, randomNumber + 1);
                }

                // 랜덤 탈출
                if (headerKey == "#ENDRANDOM")
                {
                    isRandom = false;
                    randomResult = 0;
                }

                // 조건에 대한 전처리 (#IF 숫자 / #ENDIF로 구분) // 
                // 조건 검색
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

                // 조건 탈출
                if (headerKey == "#ENDIF")
                {
                    isCheckIfstatementStarted = false;
                    isIfStatementTrue = false;
                }

                // 트랙 정보 지정
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
                        case "#BPM":
                            float.TryParse(headerValue, out trackInfo.bpm);
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
                            trackInfo.stageFile = Path.Combine(path, headerValue);
                            break;
                    }

                    // 파일 정보 관련 파싱 //
                    // 오디오
                    if (headerKey.StartsWith("#WAV"))
                    {
                        trackInfo.audioFileNames.Add(headerKey.Substring(4), Path.Combine(Directory.GetParent(path).FullName, System.Web.HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(headerValue))));
                    }
                    // BGA 이미지 (mp4도 있을수도 있음.)
                    if (headerKey.StartsWith("BMP"))
                    {
                        trackInfo.imageFileNames.Add(headerKey.Substring(4), Path.Combine(Directory.GetParent(path).FullName, System.Web.HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(headerValue))));
                    }
                }
            } while (!reader.EndOfStream);

            trackInfoList.Add(trackInfo);
        }
    }
}
