using FluentValidation;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.Validators;

public class WqxP07DataRecordValidator : AbstractValidator<WqxP07Data.RecordItem>
{
    public WqxP07DataRecordValidator()
    {
        // 測站編號 - 必填，用於關聯到監測站 (根據實體配置：最大長度50，且為唯一索引)
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

        // 地下水分區名稱 - 長度限制 (根據實體配置：最大長度100)
        RuleFor(x => x.Ugwdistname)
            .MaximumLength(100)
            .WithMessage("地下水分區名稱長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.Ugwdistname));

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

        // 使用狀態 - 長度限制 (根據實體配置：最大長度10)
        RuleFor(x => x.Statusofuse)
            .MaximumLength(10)
            .WithMessage("使用狀態長度不能超過10個字符")
            .When(x => !string.IsNullOrEmpty(x.Statusofuse));

        // 測站地址 - 長度限制（合理長度）
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

        // 坐標一致性驗證 - 使用現有的 ParseAndCorrectCoordinate 方法
        RuleFor(x => x)
            .Must(record => ValidateCoordinates(record))
            .WithMessage("TWD97坐標驗證失敗，請檢查經緯度和TM2坐標的格式和範圍")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon) || !string.IsNullOrEmpty(x.Twd97Lat) ||
                       !string.IsNullOrEmpty(x.Twd97Tm2X) || !string.IsNullOrEmpty(x.Twd97Tm2Y));
    }

    private bool ValidateCoordinates(WqxP07Data.RecordItem record)
    {
        // 如果有經緯度數據，使用 ValidateHelper.ParseAndCorrectCoordinate 進行驗證
        if (!string.IsNullOrEmpty(record.Twd97Lon) && !string.IsNullOrEmpty(record.Twd97Lat))
        {
            var result = ValidateHelper.ParseAndCorrectCoordinate(record.Twd97Lat, record.Twd97Lon);
            if (!result.IsValid)
                return false;
                
            // 檢查是否在台灣合理範圍內
            if (result.Longitude < 119 || result.Longitude > 123 ||
                result.Latitude < 21 || result.Latitude > 26)
                return false;
        }

        // 檢查坐標對的完整性（要麼都有值，要麼都沒有值）
        var hasLon = !string.IsNullOrEmpty(record.Twd97Lon);
        var hasLat = !string.IsNullOrEmpty(record.Twd97Lat);
        var hasX = !string.IsNullOrEmpty(record.Twd97Tm2X);
        var hasY = !string.IsNullOrEmpty(record.Twd97Tm2Y);

        // 經緯度必須成對出現
        if (hasLon != hasLat)
            return false;

        // TM2坐標必須成對出現
        if (hasX != hasY)
            return false;

        return true;
    }
}