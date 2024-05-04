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
            }
        }
    }
}