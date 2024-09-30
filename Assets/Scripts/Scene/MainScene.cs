public class InMainScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Game;
        Managers.UI.ShowPopupUI<UISelect>();

        return true;
    }
}
