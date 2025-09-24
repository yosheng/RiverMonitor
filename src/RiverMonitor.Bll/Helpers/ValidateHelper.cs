namespace RiverMonitor.Bll.Helpers;

public class ValidateHelper
{
    public static (bool IsValid, decimal Latitude, decimal Longitude) ParseAndCorrectCoordinate(
        string? latString,
        string? lonString)
    {
        if (!decimal.TryParse(latString, out decimal latitude) ||
            !decimal.TryParse(lonString, out decimal longitude))
        {
            return (false, 0, 0); // 解析失敗
        }

        bool isLonInRangeAsLat = longitude >= -90m && longitude <= 90m;
        bool isLatInRangeAsLon = latitude >= -180m && latitude <= 180m;

        if (isLonInRangeAsLat && isLatInRangeAsLon)
        {
            // 交換
            (latitude, longitude) = (longitude, latitude);
        }

        bool isFinalValid = latitude >= -90m && latitude <= 90m &&
                            longitude >= -180m && longitude <= 180m;

        return (isFinalValid, latitude, longitude);
    }
}