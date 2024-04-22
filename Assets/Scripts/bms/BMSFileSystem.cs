using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// BMS의 파일을 관리하는 클래스입니다.
// 유니티의 MonoBehaviour에 의존하지 않으며 대상 폴더 지정과 파일을 불러와서 헤더 정보를 딕셔너리 형태로 저장하는 역할만 합니다.

public class BMSFileSystem
{
    private Dictionary<string, List<TrackInfo>> trackInfoList = new Dictionary<string, List<TrackInfo>>();
    private Dictionary<string, Dictionary<string, List<TrackInfo>>> directoryCache = new Dictionary<string, Dictionary<string, List<TrackInfo>>>();
    private List<string> rootPaths = new List<string>();

    public BMSFileSystem()
    {
        // 지금은 카운터 없으면 임의로 지정하지만 json으로 rootPath를 저장할 수 있게 된다면 에러 반환 예정.
        if (rootPaths.Count <= 0)
        {
            rootPaths.Add(@$"{Application.dataPath}/bmsFiles");
            rootPaths.Add("C:/bmsFiles");
        }
    }
    
    public List<string> GetRootPaths()
    {
        return rootPaths;
    }

    // 이 과정에서 여기에 json 저장 예정.
    public void AddRootPaths(string path)
    {
        rootPaths.Add(path);
    }

    public Dictionary<string, List<TrackInfo>> ImportFiles(int index)
    {
        if (directoryCache.ContainsKey(rootPaths[index]))
        {
            return directoryCache[rootPaths[index]];
        }

        // load all bms files..
        List<string> extensions = new List<string> { "bms", "bme", "bml", "pms" };
        
        IEnumerable<string> bmsFilePaths = Directory
            .EnumerateFiles(rootPaths[index], "*.*", SearchOption.AllDirectories)
            .Where(s => extensions.Contains(Path.GetExtension(s).TrimStart(".").ToLowerInvariant()));

        foreach (string bmsFilePath in bmsFilePaths)
        {
            ParseSongInfoHeader(bmsFilePath);
        }

        directoryCache.Add(rootPaths[index], trackInfoList);
        trackInfoList = new Dictionary<string, List<TrackInfo>>();

        return directoryCache[rootPaths[index]];
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

            if (trackInfoList.ContainsKey(Directory.GetParent(path).Name))
            {
                trackInfoList[Directory.GetParent(path).Name].Add(trackInfo);
            }
            else {
                trackInfoList.Add(Directory.GetParent(path).Name, new List<TrackInfo>());
                trackInfoList[Directory.GetParent(path).Name].Add(trackInfo);
            }
        }
    }
}
