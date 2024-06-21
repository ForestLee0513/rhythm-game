using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BMS
{
    public abstract class ChartDecoder
    {
        public string path = default;
        private TrackInfo trackInfo = new TrackInfo();
        public TrackInfo TrackInfo { get { return trackInfo; } private set { trackInfo = value; } }
        public delegate void ParseDataDelegate(string line);
        public ParseDataDelegate parseData;

        Stack<int> randoms = new();
        Stack<int> srandoms = new();
        Stack<int> crandom = new();
        Stack<bool> skip = new();

        // 헤더에서 넘어온 랜덤 정보
        public int[] previousRandomResult { get; private set; }

        public ChartDecoder(string path)
        {
            this.path = path;
            trackInfo.path = path;
        }
        
        public ChartDecoder(TrackInfo trackInfo)
        {
            path = trackInfo.path;
            this.trackInfo = trackInfo;
        }

        public ChartDecoder(string path, int[] random)
        {
            this.path = path;
            trackInfo.path = path;
            previousRandomResult = random;
        }

        public ChartDecoder(TrackInfo trackInfo, int[] random)
        {
            path = trackInfo.path;
            this.trackInfo = trackInfo;
            previousRandomResult = random;
        }

        public void ReadFile()
        {
            randoms.Clear();
            srandoms.Clear();
            crandom.Clear();
            skip.Clear();

            using (var reader = new StreamReader(path, Encoding.GetEncoding(932)))
            {
                do
                {
                    string line = reader.ReadLine();

                    if (line.Length < 2)
                    {
                        continue;
                    }
                        
                    if (line[0] == '#')
                    {
                        bool randomOk = CheckRandomState(line, previousRandomResult);
                        if (randomOk)
                        {
                            parseData(line);
                        }
                    }

                } while (!reader.EndOfStream);

                // 이전 랜덤 결과값 저장
                if (previousRandomResult == null)
                {
                    previousRandomResult = new int[srandoms.Count];
                    var ri = srandoms.GetEnumerator();
                    for (int i = 0; i < previousRandomResult.Length; i++)
                    {
                        ri.MoveNext();
                        previousRandomResult[i] = ri.Current;
                    }
                }
            }
        }

        public bool CheckRandomState(string line, int[] selectedRandom)
        {
            string headerKey = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(0, line.IndexOf(" ")) : line;
            string headerValue = line.IndexOf(" ") > -1 && line.StartsWith("#") ? line.Substring(line.IndexOf(" ") + 1) : "";

            if (headerKey == "#RANDOM")
            {
                Int32.TryParse(headerValue, out int intStatementDataValue);
                randoms.Push(intStatementDataValue);
                if (selectedRandom != null)
                {
                    crandom.Push(selectedRandom[randoms.Count - 1]);
                }
                else
                {
                    crandom.Push(new System.Random().Next(1, intStatementDataValue + 1));
                    srandoms.Push(crandom.Peek());
                }
            }
            else if (headerKey == "#IF")
            {
                if (crandom.Any())
                {
                    Int32.TryParse(headerValue, out int intStatementDataValue);
                    skip.Push(crandom.Peek() != intStatementDataValue);
                }
            }
            else if (headerKey == "#ENDIF")
            {
                if (skip.Any())
                {
                    skip.Pop();
                }
            }
            else if (headerKey == "#ENDRANDOM")
            {
                if (crandom.Any())
                {
                    crandom.Pop();
                }
            }

            return skip.Count == 0 || skip.Peek() == false;
        }

        // Base36 //
        public static int Decode36(string str)
        {
            if (str.Length != 2) return -1;

            int result = 0;
            if (str[1] >= 'A')
                result += str[1] - 'A' + 10;
            else
                result += str[1] - '0';
            if (str[0] >= 'A')
                result += (str[0] - 'A' + 10) * 36;
            else
                result += (str[0] - '0') * 36;

            return result;
        }
    }
}
