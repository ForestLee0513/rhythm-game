using UnityEngine;

public class Lane : MonoBehaviour
{
    //        1P Index   | |         2P Index(forDP)
    //--------------------------------------------------
    //         1 3 5     | |        10  12  14
    // 6(SC)  0 2 4 8    | |       9  11  13  17 15(SC)
    
    [Header("출력 할 라인")]
    [SerializeField]
    private PatternLaneType laneType;
    // 일반노트 인 경우에만 index로 접근 시도
    [SerializeField]
    private int laneIndex;

    void Start()
    {
        if (laneType == PatternLaneType.Default)
        {
            foreach (var note in InGameManager.Instance.patternData.lines[laneIndex].NoteList)
            {
                int bar = note.Bar + 1;
                // Debug.Log($"{laneIndex}번째 마디의 {note.Beat}박자 {note.KeySound}");
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
