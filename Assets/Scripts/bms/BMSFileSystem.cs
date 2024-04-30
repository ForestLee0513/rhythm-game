using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            rootPaths.Add("C:/testBmsFile");
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
        // 경로가 없을 경우 null 반환
        if (!Directory.Exists(rootPaths[index]))
        {
            return null;
        }

        // 디렉터리 캐시가 있을 경우 캐시 결과값 출력
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
            ReadFile(bmsFilePath);
        }

        // 정렬
        foreach (string trackInfoKey in trackInfoList.Keys) 
        {
            trackInfoList[trackInfoKey].Sort();
        }

        directoryCache.Add(rootPaths[index], trackInfoList);
        trackInfoList = new Dictionary<string, List<TrackInfo>>();

        return directoryCache[rootPaths[index]];
    }

    private void ReadFile(string path)
    {
        BMSParser parser = new BMSParser(path);

        using (var reader = new StreamReader(path, Encoding.GetEncoding(932)))
        {
            do
            {
                string line = reader.ReadLine();
                parser.ParseHeader(line);
            } while (!reader.EndOfStream);

            parser.ResetRandomState();

            if (trackInfoList.ContainsKey(Directory.GetParent(path).Name))
            {
                trackInfoList[Directory.GetParent(path).Name].Add(parser.TrackInfo);
            }
            else 
            {
                trackInfoList.Add(Directory.GetParent(path).Name, new List<TrackInfo>());
                trackInfoList[Directory.GetParent(path).Name].Add(parser.TrackInfo);
            }
        }
    }
}
