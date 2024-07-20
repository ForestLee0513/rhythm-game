using BMS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��Ʈ �ʱ�ȭ
// ���� ��Ʈ ��ġ ����
// Ű���忡 �Էµ� Ű�� ���� Ű ������ ���ؼ� �´ٸ� ���κ�
// �ùٸ� ���ο��� Ÿ�̹� ���� �� �������� ����� ����ó��

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

                Debug.Log($"{note.Bar} {note.Beat} {note.KeySound} / ���õ� UI���� {i}");
            }
        }
    }

    protected override void OnTimeElapsed(double elapsedTime)
    {
        OnKeyPressEvent?.Invoke();
    }
}
