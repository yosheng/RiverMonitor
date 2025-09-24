namespace RiverMonitor.Bll.Helpers;

public static class ValidateHelper
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
    
    public static bool BeValidDecimalWithPrecision(string? value, int maxIntegerDigits, int maxDecimalDigits)
    {
        if (string.IsNullOrEmpty(value))
            return true;
        
        if (decimal.TryParse(value, out decimal decimalValue))
        {
            // 檢查是否在合理範圍內
            if (Math.Abs(decimalValue) > (decimal)Math.Pow(10, maxIntegerDigits))
                return false;
            
            // 檢查小數位數
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(decimalValue)[3])[2];
            return decimalPlaces <= maxDecimalDigits;
        }
        
        return false;
    }
    
    public static bool BeValidLongitudeWithPrecision(string? value, int maxIntegerDigits, int maxDecimalDigits)
    {
        if (string.IsNullOrEmpty(value))
            return true;
        
        if (decimal.TryParse(value, out decimal longitude))
        {
            // 範圍驗證
            if (longitude < 0 || longitude > 180)
                return false;
            
            // 精度驗證
            return ValidateHelper.BeValidDecimalWithPrecision(value, maxIntegerDigits, maxDecimalDigits);
        }
        
        return false;
    }

    public static bool BeValidLatitudeWithPrecision(string? value, int maxIntegerDigits, int maxDecimalDigits)
    {
        if (string.IsNullOrEmpty(value))
            return true;
        
        if (decimal.TryParse(value, out decimal latitude))
        {
            // 範圍驗證
            if (latitude < 0 || latitude > 90)
                return false;
            
            // 精度驗證
            return BeValidDecimalWithPrecision(value, maxIntegerDigits, maxDecimalDigits);
        }
        
        return false;
    }
    
    public static bool BeValidDateFormat(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr))
            return true;
        
        return DateTime.TryParseExact(dateStr, "yyyy-MM-dd", 
            System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out _);
    }
}