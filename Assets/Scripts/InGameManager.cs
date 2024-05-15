using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    Pattern patternData = null;

    void Start()
    {
        patternData = new BMSMainDataParser(GameManager.Instance.selectedTrack).Pattern;
        
        // for (int i = 0; i < patternData.lines.Length; ++i)
        // {
        //     foreach (Note note in patternData.lines[i].NoteList)
        //     {
        //         Debug.Log($"{i}번 째 줄의 {note.Bar} 마디의 {note.Beat}비트 {note.Flag}");
        //     }
        // }

        foreach (Note keySound in patternData.bgmKeySoundChannel)
        {
            Debug.Log($"{keySound.Bar} 마디의 {keySound.Beat} 박자에 있는 키음은 {keySound.KeySound}");
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
