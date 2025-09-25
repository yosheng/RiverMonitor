using FluentValidation;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.Validators;

public class WqxP06DataRecordValidator : AbstractValidator<WqxP06Data.RecordItem>
{
    public WqxP06DataRecordValidator()
    {
        // 測站編號 - 必填，長度限制 (根據實體配置：必填，最大長度50)
        RuleFor(x => x.Siteid)
            .NotEmpty()
            .WithMessage("測站編號為必填")
            .MaximumLength(50)
            .WithMessage("測站編號長度不能超過50個字符");

        // 測站名稱 - 長度限制 (根據實體配置：最大長度100)
        RuleFor(x => x.Sitename)
            .MaximumLength(100)
            .WithMessage("測站名稱長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.Sitename));

        // 測站英文名稱 - 長度限制 (根據實體配置：最大長度200)
        RuleFor(x => x.Siteengname)
            .MaximumLength(200)
            .WithMessage("測站英文名稱長度不能超過200個字符")
            .When(x => !string.IsNullOrEmpty(x.Siteengname));

        // 縣市 - 長度限制 (根據實體配置：最大長度50)
        RuleFor(x => x.County)
            .MaximumLength(50)
            .WithMessage("縣市長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.County));

        // 鄉鎮市區 - 長度限制 (根據實體配置：最大長度50)
        RuleFor(x => x.Township)
            .MaximumLength(50)
            .WithMessage("鄉鎮市區長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.Township));

        // 流域 - 長度限制 (根據實體配置：最大長度50)
        RuleFor(x => x.Basin)
            .MaximumLength(50)
            .WithMessage("流域長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.Basin));

        // 河川 - 長度限制 (根據實體配置：最大長度50)
        RuleFor(x => x.River)
            .MaximumLength(50)
            .WithMessage("河川長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.River));

        // 使用狀態 - 長度限制 (根據實體配置：最大長度10)
        RuleFor(x => x.Statusofuse)
            .MaximumLength(10)
            .WithMessage("使用狀態長度不能超過10個字符")
            .When(x => !string.IsNullOrEmpty(x.Statusofuse));

        // 測站地址 - 長度限制（合理長度，未在實體配置中指定）
        RuleFor(x => x.Siteaddress)
            .MaximumLength(500)
            .WithMessage("測站地址長度不能超過500個字符")
            .When(x => !string.IsNullOrEmpty(x.Siteaddress));

        // TWD97 經度 - 精度驗證 (根據實體配置：decimal(12,8))
        RuleFor(x => x.Twd97Lon)
            .Must(value => ValidateHelper.BeValidLongitudeWithPrecision(value, 4, 8))
            .WithMessage("TWD97經度格式無效，必須在0-180度之間且小數部分不能超過8位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon));

        // TWD97 緯度 - 精度驗證 (根據實體配置：decimal(12,8))
        RuleFor(x => x.Twd97Lat)
            .Must(value => ValidateHelper.BeValidLatitudeWithPrecision(value, 2, 8))
            .WithMessage("TWD97緯度格式無效，必須在0-90度之間且小數部分不能超過8位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lat));

        // TWD97 TM2 X座標 - 精度驗證 (根據實體配置：decimal(12,4))
        RuleFor(x => x.Twd97Tm2X)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 8, 4))
            .WithMessage("TWD97 TM2 X座標格式無效，必須為數字且小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Tm2X));

        // TWD97 TM2 Y座標 - 精度驗證 (根據實體配置：decimal(12,4))
        RuleFor(x => x.Twd97Tm2Y)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 8, 4))
            .WithMessage("TWD97 TM2 Y座標格式無效，必須為數字且小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Tm2Y));

        // 坐標一致性驗證 - 如果提供經緯度，必須同時提供兩個
        RuleFor(x => x)
            .Must(HaveValidCoordinatePair)
            .WithMessage("TWD97經緯度必須同時提供，不能只提供其中一個")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon) || !string.IsNullOrEmpty(x.Twd97Lat));

        // TM2坐標一致性驗證 - 如果提供TM2坐標，必須同時提供兩個
        RuleFor(x => x)
            .Must(HaveValidTm2CoordinatePair)
            .WithMessage("TWD97 TM2坐標必須同時提供，不能只提供其中一個")
            .When(x => !string.IsNullOrEmpty(x.Twd97Tm2X) || !string.IsNullOrEmpty(x.Twd97Tm2Y));

        // 台灣地區坐標範圍驗證 - 使用ValidateHelper進行台灣特定範圍驗證
        RuleFor(x => x)
            .Must(HaveValidTaiwanCoordinates)
            .WithMessage("坐標必須在台灣地區合理範圍內（經度：119-123度，緯度：21-26度）")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon) && !string.IsNullOrEmpty(x.Twd97Lat));
    }

    private bool HaveValidCoordinatePair(WqxP06Data.RecordItem record)
    {
        // 確保經緯度要麼都有值，要麼都沒有值
        var hasLon = !string.IsNullOrEmpty(record.Twd97Lon);
        var hasLat = !string.IsNullOrEmpty(record.Twd97Lat);
        return hasLon == hasLat;
    }

    private bool HaveValidTm2CoordinatePair(WqxP06Data.RecordItem record)
    {
        // 確保TM2坐標要麼都有值，要麼都沒有值
        var hasX = !string.IsNullOrEmpty(record.Twd97Tm2X);
        var hasY = !string.IsNullOrEmpty(record.Twd97Tm2Y);
        return hasX == hasY;
    }

    private bool HaveValidTaiwanCoordinates(WqxP06Data.RecordItem record)
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