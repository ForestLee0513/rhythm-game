using UnityEngine;

public class UIPopup : UIBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, true);
        // fix aspect ratio to 16:9
        SetScreenSpaceToCamera();
        FixAspectRatio();

        return true;
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }

    private Canvas canvas;
    private void SetScreenSpaceToCamera()
    {
        canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }

    public Vector2 targetResoulution = new Vector2(1920, 1080);

    public void FixAspectRatio()
    {
        float targetWidthAspect = targetResoulution.x * 0.01f;
        float targetHeightAspect = targetResoulution.y * 0.01f;

        Camera.main.aspect = targetWidthAspect / targetHeightAspect;

        float widthRatio = Screen.width / targetWidthAspect;
        float heightRatio = Screen.height / targetHeightAspect;

        float heightAdd = ((100.0f * widthRatio / heightRatio) - 100.0f) * 0.005f;
        float widthAdd = ((100.0f * heightRatio / widthRatio) - 100.0f) * 0.005f;

        if (heightRatio > widthRatio)
        {
            widthAdd = 0.0f;
        }
        else
        {
            heightAdd = 0.0f;
        }


        Camera.main.rect = new Rect(
            Camera.main.rect.x + Mathf.Abs(widthAdd),
            Camera.main.rect.y + Mathf.Abs(heightAdd),
            Camera.main.rect.width + (widthAdd * 2),
            Camera.main.rect.height + (heightAdd * 2)
            );
    }

    private void OnPreCull()
    {
        Rect rect = Camera.main.rect;
        Rect newRect = new(0, 0, 1, 1);
        Camera.main.rect = newRect;
        GL.Clear(true, true, Color.black);
        Camera.main.rect = rect;
    }
}
