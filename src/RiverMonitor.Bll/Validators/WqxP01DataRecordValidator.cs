using FluentValidation;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.Validators;

public class WqxP01DataRecordValidator : AbstractValidator<WqxP01Data.RecordItem>
{
    public WqxP01DataRecordValidator()
    {
        // 測站編號 - 必填，用於關聯到監測站
        RuleFor(x => x.Siteid)
            .NotEmpty()
            .WithMessage("測站編號為必填")
            .MaximumLength(50)
            .WithMessage("測站編號長度不能超過50個字符");

        // 測站名稱 - 長度限制（合理長度）
        RuleFor(x => x.Sitename)
            .MaximumLength(100)
            .WithMessage("測站名稱長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.Sitename));

        // 測站英文名稱 - 長度限制
        RuleFor(x => x.Siteengname)
            .MaximumLength(200)
            .WithMessage("測站英文名稱長度不能超過200個字符")
            .When(x => !string.IsNullOrEmpty(x.Siteengname));

        // 縣市 - 長度限制
        RuleFor(x => x.County)
            .MaximumLength(50)
            .WithMessage("縣市長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.County));

        // 鄉鎮市區 - 長度限制
        RuleFor(x => x.Township)
            .MaximumLength(50)
            .WithMessage("鄉鎮市區長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.Township));

        // 流域 - 長度限制
        RuleFor(x => x.Basin)
            .MaximumLength(50)
            .WithMessage("流域長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.Basin));

        // 河川 - 長度限制
        RuleFor(x => x.River)
            .MaximumLength(50)
            .WithMessage("河川長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.River));

        // TWD97 經度 - 精度驗證
        RuleFor(x => x.Twd97Lon)
            .Must(value => ValidateHelper.BeValidLongitudeWithPrecision(value, 4, 8))
            .WithMessage("TWD97經度格式無效，必須在0-180度之間且小數部分不能超過8位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon));

        // TWD97 緯度 - 精度驗證
        RuleFor(x => x.Twd97Lat)
            .Must(value => ValidateHelper.BeValidLatitudeWithPrecision(value, 2, 8))
            .WithMessage("TWD97緯度格式無效，必須在0-90度之間且小數部分不能超過8位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lat));

        // TWD97 TM2 X座標 - 精度驗證
        RuleFor(x => x.Twd97Tm2X)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 8, 4))
            .WithMessage("TWD97 TM2 X座標格式無效，必須為數字且小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Tm2X));

        // TWD97 TM2 Y座標 - 精度驗證
        RuleFor(x => x.Twd97Tm2Y)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 8, 4))
            .WithMessage("TWD97 TM2 Y座標格式無效，必須為數字且小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Tm2Y));

        // 坐標一致性驗證
        RuleFor(x => x)
            .Must(HaveValidCoordinatePair)
            .WithMessage("TWD97經緯度必須同時提供，不能只提供其中一個")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon) || !string.IsNullOrEmpty(x.Twd97Lat));

        RuleFor(x => x)
            .Must(HaveValidTm2CoordinatePair)
            .WithMessage("TWD97 TM2坐標必須同時提供，不能只提供其中一個")
            .When(x => !string.IsNullOrEmpty(x.Twd97Tm2X) || !string.IsNullOrEmpty(x.Twd97Tm2Y));

        // 台灣地區坐標範圍驗證
        RuleFor(x => x)
            .Must(HaveValidTaiwanCoordinates)
            .WithMessage("坐標必須在台灣地區合理範圍內（經度：119-123度，緯度：21-26度）")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon) && !string.IsNullOrEmpty(x.Twd97Lat));

        // 採樣日期 - 必填且格式驗證
        RuleFor(x => x.Sampledate)
            .NotEmpty()
            .WithMessage("採樣日期為必填")
            .Must(ValidateHelper.BeValidDateTimeFormat)
            .WithMessage("採樣日期格式無效，應為YYYY-MM-DD hh:mm:ss")
            .When(x => !string.IsNullOrEmpty(x.Sampledate));

        // 檢測項目 - 必填，長度限制 (根據實體配置：最大長度100)
        RuleFor(x => x.Itemname)
            .NotEmpty()
            .WithMessage("檢測項目為必填")
            .MaximumLength(100)
            .WithMessage("檢測項目名稱長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.Itemname));

        // 檢測項目英文名稱 - 長度限制 (根據實體配置：最大長度200)
        RuleFor(x => x.Itemengname)
            .MaximumLength(200)
            .WithMessage("檢測項目英文名稱長度不能超過200個字符")
            .When(x => !string.IsNullOrEmpty(x.Itemengname));

        // 檢測項目英文縮寫 - 長度限制 (根據實體配置：最大長度50)
        RuleFor(x => x.Itemengabbreviation)
            .MaximumLength(50)
            .WithMessage("檢測項目英文縮寫長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.Itemengabbreviation));

        // 檢測值 - 數值格式驗證 (根據實體配置：decimal(18,5))
        RuleFor(x => x.Itemvalue)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 13, 5))
            .WithMessage("檢測值必須為數字，整數部分不能超過13位，小數部分不能超過5位")
            .When(x => !string.IsNullOrEmpty(x.Itemvalue));

        // 檢測項目單位 - 長度限制 (根據實體配置：最大長度50)
        RuleFor(x => x.Itemunit)
            .MaximumLength(50)
            .WithMessage("檢測項目單位長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.Itemunit));

        // 備註 - 長度限制（合理長度）
        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .WithMessage("備註長度不能超過1000個字符")
            .When(x => !string.IsNullOrEmpty(x.Note));
    }

    private bool HaveValidCoordinatePair(WqxP01Data.RecordItem record)
    {
        // 確保經緯度要麼都有值，要麼都沒有值
        var hasLon = !string.IsNullOrEmpty(record.Twd97Lon);
        var hasLat = !string.IsNullOrEmpty(record.Twd97Lat);
        return hasLon == hasLat;
    }

    private bool HaveValidTm2CoordinatePair(WqxP01Data.RecordItem record)
    {
        // 確保TM2坐標要麼都有值，要麼都沒有值
        var hasX = !string.IsNullOrEmpty(record.Twd97Tm2X);
        var hasY = !string.IsNullOrEmpty(record.Twd97Tm2Y);
        return hasX == hasY;
    }

    private bool HaveValidTaiwanCoordinates(WqxP01Data.RecordItem record)
    {
        // 使用ValidateHelper進行台灣地區坐標驗證
        var result = ValidateHelper.ParseAndCorrectCoordinate(record.Twd97Lat, record.Twd97Lon);
        if (!result.IsValid)
            return false;

        // 檢查是否在台灣合理範圍內
        return result.Longitude >= 119 && result.Longitude <= 123 &&
               result.Latitude >= 21 && result.Latitude <= 26;
    }
}