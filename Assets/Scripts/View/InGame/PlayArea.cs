using UnityEngine;

public class PlayArea : MonoBehaviour
{
    [SerializeField]
    PlayAreaPresenter playAreaPresenter;

    [SerializeField]
    RectTransform[] lines = new RectTransform[18];

    private void Start()
    {
        playAreaPresenter.Init(lines);
        playAreaPresenter.OnKeyPressEvent += GetInput;
    }

    private void OnDestroy()
    {
        playAreaPresenter.OnKeyPressEvent -= GetInput;
    }

    private void GetInput()
    {
    }
}
