using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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

        Stack<int> randomStack = new Stack<int>();
        Stack<bool> skipStack = new Stack<bool>();
        Stack<int> currentRandomCache = new Stack<int>();

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

        public void ReadFile()
        {
            if (trackInfo.md5 == null || trackInfo.md5 == "")
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var hash = md5.ComputeHash(stream);
                        trackInfo.md5 = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }

            StringBuilder parsedLine = new StringBuilder();
            // ?? ???
            // TODO: Random???? ?? ???? ??? ???? (??? ??? ???? ??? TrackInfo? ???.)
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
                        string headerKey = line.IndexOf(" ") > -1 ? line.Substring(0, line.IndexOf(" ")) : line;
                        string headerValue = line.IndexOf(" ") > -1 ? line.Substring(line.IndexOf(" ") + 1) : "";

                        
                        if (headerKey == "#IF")
                        {
                            if (randomStack.Count == 0)
                            {
                                Console.WriteLine("RandomStack is empty!");
                                continue;
                            }

                            int currentRandom = randomStack.Peek();
                            Int32.TryParse(headerValue, out int n);
                            skipStack.Push(currentRandom != n);
                            continue;
                        }
                        if (headerKey == "#ELSE")
                        {
                            if (skipStack.Count == 0)
                            {
                                Console.WriteLine("SkipStack is empty!");
                                continue;
                            }
                            bool currentSkip = skipStack.Pop();
                            skipStack.Push(!currentSkip);
                            continue;
                        }
                        if (headerKey == "ELSEIF")
                        {
                            if (skipStack.Count == 0)
                            {
                                Console.WriteLine("SkipStack is empty!");
                                continue;
                            }
                            bool currentSkip = skipStack.Pop();
                            int currentRandom = randomStack.Peek();
                            Int32.TryParse(headerValue, out int n);
                            skipStack.Push(currentSkip && currentRandom != n);
                            continue;
                        }
                        if (headerKey == "#ENDIF")
                        {
                            if (skipStack.Count == 0)
                            {
                                Console.WriteLine("SkipStack is empty!");
                                continue;
                            }
                            skipStack.Pop();
                            continue;
                        }
                        if (skipStack.Count > 0 && skipStack.Peek() == true)
                        {
                            continue;
                        }
                        if (headerKey == "#RANDOM")
                        {
                            if (trackInfo.selectedRandom == null)
                            {
                                Int32.TryParse(headerValue, out int n);
                                int randomResult = new System.Random().Next(1, n + 1);
                                randomStack.Push(randomResult);
                                currentRandomCache.Push(randomResult);
                                continue;
                            }
                            else
                            {
                                for (int i = trackInfo.selectedRandom.Length - 1; i > -1; --i)
                                {
                                    int result = trackInfo.selectedRandom[i];
                                    randomStack.Push(result);
                                    continue;
                                }
                            }
                        }
                        if (headerKey == "#ENDRANDOM")
                        {
                            if (randomStack.Count == 0)
                            {
                                Console.WriteLine("RandomStack is empty!");
                                continue;
                            }

                            randomStack.Pop();
                            continue;
                        }

                        parseData(line);
                    }
                } while (!reader.EndOfStream);
            }

            if (trackInfo.selectedRandom == null)
            {
                trackInfo.selectedRandom = currentRandomCache.ToArray();
            }
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
