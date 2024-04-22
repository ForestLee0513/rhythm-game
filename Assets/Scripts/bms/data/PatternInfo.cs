using System.Reflection;

public class PatternInfo : TrackInfo
{
    public PatternInfo (TrackInfo trackInfo)
    {
        if (trackInfo is TrackInfo)
        {
            PropertyInfo[] properties = trackInfo.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                UnityEngine.Debug.Log(property);
            }
        }
    }
}
