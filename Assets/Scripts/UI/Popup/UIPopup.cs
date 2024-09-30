using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : UIBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, true);

        return true;
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
