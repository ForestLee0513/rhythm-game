using System;
using UnityEngine;
using UnityEngine.UI;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Utils.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action action, UIDefine.UIEvent type = UIDefine.UIEvent.Click)
    {
        UIBase.BindEvent(go, action, type);
    }

    public static void ResetVertical(this ScrollRect scrollRect)
    {
        scrollRect.verticalNormalizedPosition = 1;
    }

    public static void ResetHorizontal(this ScrollRect scrollRect)
    {
        scrollRect.horizontalNormalizedPosition = 1;
    }
}
