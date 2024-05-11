using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class BMSMainDataParser : ChartDecoder
{
    #region Random Statements
    bool isRandom = false;
    bool isIfStatementTrue = false;
    bool isCheckIfstatementStarted = false;
    int randomResult = 0;
    #endregion
    private Pattern pattern = new Pattern();
    public Pattern Pattern { get { return pattern; }}

    public BMSMainDataParser(string path) : base(path)
    {
        parseData += ParseMainData;
        ReadFile();
    }

    public BMSMainDataParser(TrackInfo trackInfo): base(trackInfo)
    {
        parseData += ParseMainData;
        ReadFile();
    }

    private void ParseMainData(string line)
    {
        // 메인 데이터 형식 (#001XX:AABBCC)이 아닐경우 파싱 생략
        if (line == "" || line.IndexOf(" ") > -1)
        {
            return;
        }

        string statementDataKey = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(0, line.IndexOf(" ")) : line;
        string statementDataValue = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(" ") + 1) : "";

        string mainDataKey = line.IndexOf(":") > -1 && line.StartsWith("#") ? line.Substring(1, line.IndexOf(":") - 1) : "";
        string mainDataValue = line.IndexOf(":") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(":") + 1) : "";

        // Random에 대한 전처리 (#RANDOM 숫자 / #ENDRANDOM으로 구분) //
        // 랜덤 값 지정
        if (statementDataKey == "#RANDOM")
        {
            isRandom = true;
            Int32.TryParse(statementDataValue, out int randomNumber);
            randomResult = new System.Random().Next(1, randomNumber + 1);
        }

        // 랜덤 탈출
        if (statementDataKey == "#ENDRANDOM")
        {
            isRandom = false;
            randomResult = 0;
        }

        // 조건에 대한 전처리 (#IF 숫자 / #ENDIF로 구분) // 
        // 조건 검색
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

        // 조건 탈출
        if (statementDataKey == "#ENDIF")
        {
            isCheckIfstatementStarted = false;
            isIfStatementTrue = false;
        }

        if (isRandom == false || (isIfStatementTrue == true && isCheckIfstatementStarted == true) || isCheckIfstatementStarted == false)
        {
            if (mainDataKey != "" && mainDataValue != "")
            {
                // 마디
                Int32.TryParse(mainDataKey.Substring(0, 3), out int bar);
                
                if (pattern.bar < bar)
                {
                    pattern.bar = bar;
                }

                string channel = mainDataKey.Substring(3);
                int lane = channel[1] - '0'; // 2P거나 DP일 경우 8 더해서 둘 다 출력할 수 있도록 수정 예정


                // 롱노트 시작 / 종료 여부
                bool isLntypeStarted = false;
                int prevLnBar = 0;
                int prevLnBeat = 0;

                // 채널 02를 제외하고 모두 36진수로 이루어져 있어 채널 여부로 구분
                if (channel != "02")
                {
                    int beatLength = mainDataValue.Length / 2;

                    for (int i = 0; i < mainDataValue.Length - 1; i += 2)
                    {
                        // 키음
                        int keySound = Decode36(mainDataValue.Substring(i, 2));

                        // 키음이 00이 아닐때만 노트, BGA 등 에셋 배치
                        if (keySound == 0)
                        {
                            continue;
                        }

                        // 노트 처리 //
                        if (channel[0] == '1' || channel[0] == '2')
                        {
                            // 롱노트 - LNOBJ 선언 됐을 경우의 처리
                            if (TrackInfo.lnobj == keySound)
                            {
                                Debug.Log($"롱노트 시작지점은 {prevLnBar}번째 마디의 {lane}번 라인 {prevLnBeat} 비트에 있음 / 키음은 이미 리스트에 적용되어 있으니 생략.");
                                Debug.Log($"롱노트 끝지점은 {bar}번째 마디의 {lane}번 라인 {i}번째 비트에 있음 / 키음은 :{keySound}");

                                continue;
                            }

                            // 일반 노트
                            Debug.Log($"{bar}번째 마디의 {lane}번 라인 {i} 비트에 일반노트 키음은 :{keySound}");
                            prevLnBar = bar;
                            prevLnBeat = i;
                            continue;
                        }
                        
                        // 롱노트 - LNTYPE 1 명령어 일 경우의 처리
                        if (TrackInfo.lnType == 1 && (channel[0] == '5' || channel[0] == '6'))
                        {
                            if (isLntypeStarted == true)
                            {
                                Debug.Log($"롱노트 끝지점은 {bar}번째 마디의 {lane}번 라인 {i}번째 비트에 있음 / 키음은 :{keySound}");
                                isLntypeStarted = false;
                                continue;
                            }

                            Debug.Log($"롱노트 시작지점은 {bar}번째 마디의 {lane}번 라인 {i}번째 비트에 있음 / 키음은: {keySound}");
                            isLntypeStarted = !isLntypeStarted;
                            continue;
                        }
                    }
                }
                else
                {
                    // 변박 //
                    // 마디 내 박자 수 (1이 4/4 박자임.)
                    double.TryParse(mainDataValue, out double beatMeter);
                    Debug.Log($"{bar}번 째 마디의 박자는 {beatMeter}");
                }
            }
        }
    }
}
