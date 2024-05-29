using UnityEngine;
using System.Collections.Generic;

public class Lane : MonoBehaviour
{
    //        1P Index   | |         2P Index(forDP)
    //--------------------------------------------------
    //         1 3 5     | |        10  12  14
    // 6(SC)  0 2 4 8    | |       9  11  13  17 15(SC)
    
    [Header("출력 할 라인")]
    [SerializeField]
    private BMS.PatternLaneType laneType;
    // 일반노트 인 경우에만 index로 접근 시도
    [SerializeField]
    private int laneIndex;
    [Header("노트 에셋")]
    [SerializeField]
    private Note notePrefab;
    private Dictionary<int, List<Note>> noteMap = new();

    void Start()
    {
        if (laneType == BMS.PatternLaneType.Default)
        {
            foreach (var note in InGameManager.Instance.patternData.lines[laneIndex].NoteList)
            {
                int bar = note.Bar;
                Vector3 notePos = new Vector3(0, (float)note.Beat);

            }
        }
        else
        {
            
        }
    }

    void Update()
    {
        
    }
}
