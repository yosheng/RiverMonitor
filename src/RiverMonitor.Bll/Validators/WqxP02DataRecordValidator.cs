using FluentValidation;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.Validators;

public class WqxP02DataRecordValidator : AbstractValidator<WqxP02Data.RecordItem>
{
    public WqxP02DataRecordValidator()
    {
        // 測站編號 - 必填，用於關聯到監測站 (根據實體配置：外鍵關聯)
        RuleFor(x => x.Siteid)
            .NotEmpty()
            .WithMessage("測站編號為必填")
            .MaximumLength(50)
            .WithMessage("測站編號長度不能超過50個字符");

        // 測站名稱 - 長度限制（驗證一致性）
        RuleFor(x => x.Sitename)
            .MaximumLength(100)
            .WithMessage("測站名稱長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.Sitename));

        // 測站英文名稱 - 長度限制
        RuleFor(x => x.Siteengname)
            .MaximumLength(200)
            .WithMessage("測站英文名稱長度不能超過200個字符")
            .When(x => !string.IsNullOrEmpty(x.Siteengname));

        // 地下水分區名稱 - 長度限制
        RuleFor(x => x.Ugwdistname)
            .MaximumLength(100)
            .WithMessage("地下水分區名稱長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.Ugwdistname));

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

        // TWD97 經度 - 精度驗證 (與監測站基本資料一致)
        RuleFor(x => x.Twd97Lon)
            .Must(value => ValidateHelper.BeValidLongitudeWithPrecision(value, 4, 8))
            .WithMessage("TWD97經度格式無效，必須在0-180度之間且小數部分不能超過8位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon));

        // TWD97 緯度 - 精度驗證 (與監測站基本資料一致)
        RuleFor(x => x.Twd97Lat)
            .Must(value => ValidateHelper.BeValidLatitudeWithPrecision(value, 2, 8))
            .WithMessage("TWD97緯度格式無效，必須在0-90度之間且小數部分不能超過8位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lat));

        // TWD97 TM2 X座標 - 精度驗證 (與監測站基本資料一致)
        RuleFor(x => x.Twd97Tm2X)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 8, 4))
            .WithMessage("TWD97 TM2 X座標格式無效，必須為數字且小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Tm2X));

        // TWD97 TM2 Y座標 - 精度驗證 (與監測站基本資料一致)
        RuleFor(x => x.Twd97Tm2Y)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 8, 4))
            .WithMessage("TWD97 TM2 Y座標格式無效，必須為數字且小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.Twd97Tm2Y));

        // 採樣日期 - 必填且格式驗證 (根據實體配置：SampleDate 為 DateTime)
        RuleFor(x => x.Sampledate)
            .NotEmpty()
            .WithMessage("採樣日期為必填")
            .Must(ValidateHelper.BeValidDateTimeFormat)
            .WithMessage("採樣日期格式無效，應為YYYY-MM-DD hh:mm:ss")
            .When(x => !string.IsNullOrEmpty(x.Sampledate));

        // 檢測項目名稱 - 長度限制 (根據實體配置：最大長度100)
        RuleFor(x => x.Itemname)
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

        // 坐標一致性驗證 - 重用 ValidateHelper 邏輯
        RuleFor(x => x)
            .Must(record => ValidateCoordinates(record))
            .WithMessage("TWD97坐標驗證失敗，請檢查經緯度和TM2坐標的格式和範圍")
            .When(x => !string.IsNullOrEmpty(x.Twd97Lon) || !string.IsNullOrEmpty(x.Twd97Lat) ||
                       !string.IsNullOrEmpty(x.Twd97Tm2X) || !string.IsNullOrEmpty(x.Twd97Tm2Y));
    }

    private bool ValidateCoordinates(WqxP02Data.RecordItem record)
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