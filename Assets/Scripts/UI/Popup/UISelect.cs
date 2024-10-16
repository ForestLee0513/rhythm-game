using BMS;

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

        BMSModel bmsModel = new BMSDecoder().Decode("D:\\BMSFiles\\[clover]LeaF_Aleph0\\_7ANOTHER.bms");

        UnityEngine.Debug.Log(bmsModel.Title);
        UnityEngine.Debug.Log(bmsModel.Artist);


        return true;
    }
}
