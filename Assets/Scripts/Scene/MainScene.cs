public class InMainScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = UIDefine.Scene.Game;
        Managers.UI.ShowPopupUI<UISelect>();

        return true;
    }
}
