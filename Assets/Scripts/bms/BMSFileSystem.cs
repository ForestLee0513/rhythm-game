using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BMSFileSystem : MonoBehaviour
{
    private string rootPath;
    private List<TrackInfo> trackInfoList = new List<TrackInfo>();

    private void Awake()
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
        List<string> extensions = new List<string> { "bms", "bme", "pms" };
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

        trackInfo.path = path;
        using (var reader = new StreamReader(path))
        {
            do
            {
                string line = reader.ReadLine();

                // 헤더 필드가 아닐 때는 다음 파일로 이동
                if (line.StartsWith("*----------------------") && line != "*---------------------- HEADER FIELD")
                {
                    break;
                }

                // 헤더 필드 시작 영역, 공백 생략
                if (line == "" || line == "*---------------------- HEADER FIELD")
                {
                    continue;
                }

                string headerKey = line.Substring(0, line.IndexOf(' '));
                string headerValue = line.Substring(line.IndexOf(" ") + 1);

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
                        Int32.TryParse(headerValue, out trackInfo.bpm);
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
                    default:
                        // 키 사운드, BGA 프레임 등 노래 정보와 상관없는 헤더 정보일 때는 다음 파일로 생략
                        continue;
                }
            } while (!reader.EndOfStream);

            trackInfoList.Add(trackInfo);
        }
    }
}
