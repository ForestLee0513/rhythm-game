using BMSParser;
using UnityEngine;

public class UISelect : UIPopup
{
    enum Texts
    {

    }

    enum Buttons
    {

    }

    enum Images
    {

    }



    public override bool Init()     
    {
        if (base.Init() == false)
            return false;

        BMSModel bmsModel = new BMS().Decode("D:\\bms\\7key\\[Clue]Random\\_random_s4.bms");

        Debug.Log(bmsModel.Title);
        Debug.Log(bmsModel.Artist);

        return true;
    }
}
