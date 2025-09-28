using FluentValidation;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.Validators;

public class WaterQualityOpenDataValidator : AbstractValidator<WaterQualityOpenData>
{
    public WaterQualityOpenDataValidator()
    {
        // 管理處名稱 - 必填
        RuleFor(x => x.ManagementOfficeName)
            .NotEmpty()
            .WithMessage("管理處名稱為必填")
            .MaximumLength(100)
            .WithMessage("管理處名稱長度不能超過100個字符");

        // 工作站 - 必填
        RuleFor(x => x.Workstation)
            .NotEmpty()
            .WithMessage("工作站為必填")
            .MaximumLength(100)
            .WithMessage("工作站名稱長度不能超過100個字符");

        // 站點名稱 - 必填
        RuleFor(x => x.SiteName)
            .NotEmpty()
            .WithMessage("站點名稱為必填")
            .MaximumLength(200)
            .WithMessage("站點名稱長度不能超過200個字符");

        // 採樣日期 - 必填且格式驗證
        RuleFor(x => x.SampleDate)
            .NotEmpty()
            .WithMessage("採樣日期為必填")
            .Must(BeValidDateFormat)
            .WithMessage("採樣日期格式無效，應為YYYY/MM/DD")
            .When(x => !string.IsNullOrEmpty(x.SampleDate));

        // 水溫 - 數值格式驗證（可選）
        RuleFor(x => x.WaterTemperatureC)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 3, 2))
            .WithMessage("水溫必須為有效數字，整數部分不能超過3位，小數部分不能超過2位")
            .When(x => !string.IsNullOrEmpty(x.WaterTemperatureC));

        // pH值 - 數值格式和範圍驗證（可選）
        RuleFor(x => x.PhValue)
            .Must(BeValidPhValue)
            .WithMessage("pH值必須為0-14之間的有效數字")
            .When(x => !string.IsNullOrEmpty(x.PhValue));

        // 電導度 - 數值格式驗證（可選）
        RuleFor(x => x.ElectricalConductivity)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 8, 2))
            .WithMessage("電導度必須為有效數字，整數部分不能超過8位，小數部分不能超過2位")
            .When(x => !string.IsNullOrEmpty(x.ElectricalConductivity));

        // 備註 - 長度限制（可選）
        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .WithMessage("備註長度不能超過1000個字符")
            .When(x => !string.IsNullOrEmpty(x.Note));

        // 版本 - 長度限制（可選）
        RuleFor(x => x.Version)
            .MaximumLength(50)
            .WithMessage("版本長度不能超過50個字符")
            .When(x => !string.IsNullOrEmpty(x.Version));
    }
    
    private static bool BeValidDateFormat(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr))
            return true;
        
        return DateTime.TryParseExact(dateStr, "yyyy/MM/dd", 
            System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out _);
    }

    private bool BeValidPhValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return true;

        if (decimal.TryParse(value, out decimal phValue))
        {
            // pH值通常在0-14之間
            return phValue >= 0 && phValue <= 14;
        }

        return false;
    }
}