using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    BMSParser trackData = null;


    void Start()
    {
        trackData = new BMSParser(GameManager.Instance.selectedTrack.path);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
