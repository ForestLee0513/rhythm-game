using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    Pattern patternData = null;

    void Start()
    {
        patternData = new BMSMainDataParser(GameManager.Instance.selectedTrack).Pattern;
        Debug.Log(patternData.bar);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
