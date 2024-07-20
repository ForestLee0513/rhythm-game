using BMS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 노트 초기화
// 현재 노트 위치 지정
// 키보드에 입력된 키와 현재 키 설정을 비교해서 맞다면 라인비교
// 올바른 라인에서 타이밍 검증 후 판정범위 내라면 판정처리

public class PlayAreaPresenter : BMSObjectObserver
{
    public event Action OnKeyPressEvent;
    private List<GameObject>[] createdLine = new List<GameObject>[18];

    public void Init(RectTransform[] playAreaLane)
    {
        for (int i = 0; i < InGameManager.Instance.patternData.lines.Length; ++i)
        {
            for (int j = 0; j < InGameManager.Instance.patternData.lines[i].NoteList.Count; ++j)
            {
                Note note = InGameManager.Instance.patternData.lines[i].NoteList[j];
                RectTransform noteParent = playAreaLane[i];
                
                GameObject newNote = new GameObject();
                newNote.AddComponent<RectTransform>();
                RectTransform newNoteRectTransform = newNote.GetComponent<RectTransform>();
                newNote.transform.SetParent(noteParent.transform, false);
                newNote.transform.position = new Vector2(newNote.transform.position.x, (float)note.Beat);
                newNoteRectTransform.sizeDelta = noteParent.sizeDelta;
                newNote.AddComponent<Image>();

                // caching
                if (createdLine[i] == null)
                {
                    createdLine[i] = new List<GameObject>();
                }

                createdLine[i].Add(newNote);

                Debug.Log($"{note.Bar} {note.Beat} {note.KeySound} / 선택된 UI라인 {i}");
            }
        }
    }

    protected override void OnTimeElapsed(double elapsedTime)
    {
        OnKeyPressEvent?.Invoke();
    }
}
