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
    private List<TrackInfo> trackInfoList = new List<TrackInfo>();

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

        foreach (TrackInfo trackInfo in trackInfoList)
        {
            Debug.Log(trackInfo.title + " / " + trackInfo.artist + " / " + trackInfo.genre);
        }
    }

    private void ParseSongInfoHeader(string path)
    {
        TrackInfo trackInfo = new TrackInfo();
        bool isRandom = false;
        bool CheckIfStatement = false;
        int randomResult = 0;

        trackInfo.path = path;
        using (var reader = new StreamReader(path))
        {
            do
            {
                string line = reader.ReadLine();

                // 헤더 필드가 아닐 때는 다음 파일로 이동
                if ((line.StartsWith("*----------------------") && line != "*---------------------- HEADER FIELD"))
                {
                    break;
                }

                string headerKey = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(0, line.IndexOf(" ")) : line;
                string headerValue = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(" ") + 1) : "";

                // Random에 대한 전처리 (#RANDOM 숫자 / #ENDRANDOM으로 구분) //
                // 랜덤 값 지정
                if (headerKey == "#RANDOM" && isRandom == false)
                {
                    isRandom = true;
                    Int32.TryParse(headerValue, out int randomNumber);
                    randomResult = UnityEngine.Random.Range(1, randomNumber + 1);
                }

                // 랜덤 탈출
                if (headerKey == "#ENDRANDOM" && isRandom == true)
                {
                    isRandom = false;
                    randomResult = 0;
                }

                // 조건에 대한 전처리 (#IF 숫자 / #ENDIF로 구분) // 
                // 조건 검색
                if (headerKey == "#IF" && Int32.TryParse(headerValue, out int parsedHeaderValueNumber) && isRandom == true)
                {
                    if (parsedHeaderValueNumber == randomResult)
                    {
                        CheckIfStatement = true;
                    }
                    else
                    {
                        CheckIfStatement = false;
                    }
                }

                // 조건 탈출
                if(headerKey == "#ENDIF")
                {
                    CheckIfStatement = false;
                }

                // Start 랜덤 여부가 false일 때 실행되는 기본 처리 //
                if (isRandom == false || CheckIfStatement == true)
                {
                    SetTrackInfoValue(headerKey, headerValue, trackInfo, path);
                }
            } while (!reader.EndOfStream);

            trackInfoList.Add(trackInfo);
        }
    }

    private void SetTrackInfoValue(string headerKey, string headerValue, TrackInfo trackInfo, string path)
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
            case "#DIFFICULTY":
                Int32.TryParse(headerValue, out trackInfo.difficulty);
                break;
            case "#RANK":
                Int32.TryParse(headerValue, out trackInfo.rank);
                break;
            case "#TOTAL":
                Int32.TryParse(headerValue, out trackInfo.total);
                break;
            case "#STAGEFILE":
                trackInfo.stageFile = Path.Combine(path, headerValue);
                break;
        }
    }
}
