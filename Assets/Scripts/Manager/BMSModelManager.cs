using BMSParser;
using System;
using System.IO;
using UnityEngine;

public class BMSModelManager
{
    int randomSeed = 0;
    BMSModel loadedModel;

    public BMSModelManager()
    {
        randomSeed = Environment.TickCount;
    }

    public void LoadBMS(string path)
    {
        try
        {
            loadedModel = new BMS().Decode(path, randomSeed);
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void SetRandomSeed(int newRandomSeed)
    {
        randomSeed = newRandomSeed;
    }
}
