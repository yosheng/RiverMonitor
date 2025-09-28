using FluentValidation;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.Validators;

public class EmsS07DataRecordValidator : AbstractValidator<EmsS07Data.RecordItem>
{
    public EmsS07DataRecordValidator()
    {
        // 場址代碼 - 必填，長度限制
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage("場址代碼為必填")
            .MaximumLength(50)
            .WithMessage("場址代碼長度不能超過50個字符");

        // 場址名稱 - 必填，長度限制
        RuleFor(x => x.SiteName)
            .NotEmpty()
            .WithMessage("場址名稱為必填")
            .MaximumLength(255)
            .WithMessage("場址名稱長度不能超過255個字符");

        // 公告管制文號 - 長度限制（可選）
        RuleFor(x => x.AnnoNo)
            .MaximumLength(100)
            .WithMessage("公告管制文號長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.AnnoNo));

        // 公告解除文號 - 長度限制（可選）
        RuleFor(x => x.DeannoNo)
            .MaximumLength(100)
            .WithMessage("公告解除文號長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.DeannoNo));

        // 縣市 - 長度限制（可選）
        RuleFor(x => x.County)
            .MaximumLength(50)
            .WithMessage("縣市長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.County));

        // 場址類型 - 長度限制（可選）
        RuleFor(x => x.SiteType)
            .MaximumLength(100)
            .WithMessage("場址類型長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.SiteType));

        // 場址地址 - 長度限制（可選）
        RuleFor(x => x.Pollutantaddress)
            .MaximumLength(500)
            .WithMessage("場址地址長度不能超過500個字符")
            .When(x => !string.IsNullOrEmpty(x.Pollutantaddress));

        // 場址管制類型 - 長度限制（可選）
        RuleFor(x => x.Controltype)
            .MaximumLength(100)
            .WithMessage("場址管制類型長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.Controltype));

        // 鄉鎮市區 - 長度限制（可選）
        RuleFor(x => x.Township)
            .MaximumLength(50)
            .WithMessage("鄉鎮市區長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.Township));

        // 地段地號 - 長度限制（可選）
        RuleFor(x => x.Landno)
            .MaximumLength(1000)
            .WithMessage("地段地號長度不能超過1000個字符")
            .When(x => !string.IsNullOrEmpty(x.Landno));

        // 日期格式驗證（可選字段）
        RuleFor(x => x.AnnoDate)
            .Must(ValidateHelper.BeValidDateFormat)
            .WithMessage("公告管制日期格式無效，應為YYYY-MM-DD")
            .When(x => !string.IsNullOrEmpty(x.AnnoDate));

        RuleFor(x => x.DeannoDate)
            .Must(ValidateHelper.BeValidDateFormat)
            .WithMessage("公告解除日期格式無效，應為YYYY-MM-DD")
            .When(x => !string.IsNullOrEmpty(x.DeannoDate));

        // 數值格式驗證
        // 場址面積 - 必須為正數
        RuleFor(x => x.Sitearea)
            .Must(BeValidPositiveDecimal)
            .WithMessage("場址面積必須為正數")
            .When(x => !string.IsNullOrEmpty(x.Sitearea));

        // TM2X座標 - decimal(18,6) - 整數部分最大12位，小數部分6位
        RuleFor(x => x.Dtmx)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 12, 6))
            .WithMessage("TM2X座標格式無效，整數部分不能超過12位，小數部分不能超過6位")
            .When(x => !string.IsNullOrEmpty(x.Dtmx));

        // TM2Y座標 - decimal(18,6) - 整數部分最大12位，小數部分6位
        RuleFor(x => x.Dtmy)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 12, 6))
            .WithMessage("TM2Y座標格式無效，整數部分不能超過12位，小數部分不能超過6位")
            .When(x => !string.IsNullOrEmpty(x.Dtmy));

        // WGS84經度 - 經緯度範圍驗證 + 精度驗證
        RuleFor(x => x.Wgs84Lng)
            .Must(value => ValidateHelper.BeValidLongitudeWithPrecision(value, 3, 15))
            .WithMessage("WGS84經度格式無效，必須在0-180度之間")
            .When(x => !string.IsNullOrEmpty(x.Wgs84Lng));

        // WGS84緯度 - 經緯度範圍驗證 + 精度驗證
        RuleFor(x => x.Wgs84Lat)
            .Must(value => ValidateHelper.BeValidLatitudeWithPrecision(value, 3, 15))
            .WithMessage("WGS84緯度格式無效，必須在0-90度之間")
            .When(x => !string.IsNullOrEmpty(x.Wgs84Lat));

        // 污染物質及濃度 - 長度限制（可選）
        RuleFor(x => x.Pollutant)
            .MaximumLength(4000)
            .WithMessage("污染物質及濃度描述長度不能超過4000個字符")
            .When(x => !string.IsNullOrEmpty(x.Pollutant));
    }
    
    private bool BeValidPositiveDecimal(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return true;
        
        if (decimal.TryParse(value, out decimal decimalValue))
        {
            return decimalValue > 0;
        }
        
        return false;
    }
}