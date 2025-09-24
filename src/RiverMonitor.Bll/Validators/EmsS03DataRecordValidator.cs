using FluentValidation;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.Validators;

public class EmsS03DataRecordValidator : AbstractValidator<EmsS03Data.RecordItem>
{
    public EmsS03DataRecordValidator()
    {
        // 管制編號 - 必填，長度限制
        RuleFor(x => x.EmsNo)
            .NotEmpty()
            .WithMessage("管制事業編號為必填")
            .MaximumLength(50)
            .WithMessage("管制事業編號長度不能超過50個字符");

        // 事業名稱 - 必填，長度限制
        RuleFor(x => x.FacName)
            .NotEmpty()
            .WithMessage("事業名稱為必填")
            .MaximumLength(255)
            .WithMessage("事業名稱長度不能超過255個字符");

        // 地址 - 長度限制（可選）
        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("地址長度不能超過500個字符")
            .When(x => !string.IsNullOrEmpty(x.Address));

        // 統一編號 - 格式和長度驗證（可選）
        RuleFor(x => x.Unino)
            .Matches(@"^\d{8}$")
            .WithMessage("統一編號必須為8位數字")
            .MaximumLength(10)
            .WithMessage("統一編號長度不能超過10個字符")
            .When(x => !string.IsNullOrEmpty(x.Unino));

        // 許可證號 - 長度限制（可選）
        RuleFor(x => x.PerNo)
            .MaximumLength(100)
            .WithMessage("許可證號長度不能超過100個字符")
            .When(x => !string.IsNullOrEmpty(x.PerNo));

        // 日期格式驗證（可選字段）
        RuleFor(x => x.PerSdate)
            .Must(ValidateHelper.BeValidDateFormat)
            .WithMessage("許可證起始日格式無效，應為YYYY-MM-DD")
            .When(x => !string.IsNullOrEmpty(x.PerSdate));

        RuleFor(x => x.PerEdate)
            .Must(ValidateHelper.BeValidDateFormat)
            .WithMessage("許可證截止日格式無效，應為YYYY-MM-DD")
            .When(x => !string.IsNullOrEmpty(x.PerEdate));

        RuleFor(x => x.EmiSdate)
            .Must(ValidateHelper.BeValidDateFormat)
            .WithMessage("申報區間起始日格式無效，應為YYYY-MM-DD")
            .When(x => !string.IsNullOrEmpty(x.EmiSdate));

        RuleFor(x => x.EmiEdate)
            .Must(ValidateHelper.BeValidDateFormat)
            .WithMessage("申報區間截止日格式無效，應為YYYY-MM-DD")
            .When(x => !string.IsNullOrEmpty(x.EmiEdate));

        // 數值格式驗證 - 嚴格按照DB精度要求
        RuleFor(x => x.PigTot)
            .Must(BeValidInteger)
            .WithMessage("養豬頭數必須為整數")
            .When(x => !string.IsNullOrEmpty(x.PigTot));

        // decimal(18,4) - 整數部分最大14位，小數部分4位
        RuleFor(x => x.PerWater)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 14, 4))
            .WithMessage("廢水處理設施處理水量必須為數字，整數部分不能超過14位，小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.PerWater));

        RuleFor(x => x.PerRecycle)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 14, 4))
            .WithMessage("回收量必須為數字，整數部分不能超過14位，小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.PerRecycle));

        RuleFor(x => x.PerStay)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 14, 4))
            .WithMessage("貯留水量必須為數字，整數部分不能超過14位，小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.PerStay));

        RuleFor(x => x.PerTrustee)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 14, 6))
            .WithMessage("受託處理水量必須為數字，整數部分不能超過14位，小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.PerTrustee));

        RuleFor(x => x.PerDelegate)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 14, 6))
            .WithMessage("委託處理水量必須為數字，整數部分不能超過14位，小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.PerDelegate));

        RuleFor(x => x.EmiWater)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 14, 6))
            .WithMessage("排放水量必須為數字，整數部分不能超過14位，小數部分不能超過4位")
            .When(x => !string.IsNullOrEmpty(x.EmiWater));

        // 排放濃度 - decimal(18,10) - 整數部分最大8位，小數部分10位
        RuleFor(x => x.EmiValue)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 8, 10))
            .WithMessage("排放濃度必須為數字，整數部分不能超過8位，小數部分不能超過10位")
            .When(x => !string.IsNullOrEmpty(x.EmiValue));

        // 污染量 - decimal(18,10) - 整數部分最大8位，小數部分10位
        RuleFor(x => x.ItemValue)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 16, 10))
            .WithMessage("污染量必須為數字，整數部分不能超過16位，小數部分不能超過10位")
            .When(x => !string.IsNullOrEmpty(x.ItemValue));

        // 座標驗證 - decimal(18,6) - 整數部分最大12位，小數部分6位
        RuleFor(x => x.LetTm2x)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 12, 6))
            .WithMessage("放流口X座標格式無效，整數部分不能超過12位，小數部分不能超過6位")
            .When(x => !string.IsNullOrEmpty(x.LetTm2x));

        RuleFor(x => x.LetTm2y)
            .Must(value => ValidateHelper.BeValidDecimalWithPrecision(value, 12, 6))
            .WithMessage("放流口Y座標格式無效，整數部分不能超過12位，小數部分不能超過6位")
            .When(x => !string.IsNullOrEmpty(x.LetTm2y));

        // 經緯度 - decimal(18,10) - 整數部分最大8位，小數部分10位
        RuleFor(x => x.LetEast)
            .Must(value => ValidateHelper.BeValidLongitudeWithPrecision(value, 8, 10))
            .WithMessage("東經格式無效，必須在0-180度之間，整數部分不能超過8位，小數部分不能超過10位")
            .When(x => !string.IsNullOrEmpty(x.LetEast));

        RuleFor(x => x.LetNorth)
            .Must(value => ValidateHelper.BeValidLatitudeWithPrecision(value, 8, 10))
            .WithMessage("北緯格式無效，必須在0-90度之間，整數部分不能超過8位，小數部分不能超過10位")
            .When(x => !string.IsNullOrEmpty(x.LetNorth));
    }
    
    private bool BeValidInteger(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return true;
        
        return int.TryParse(value, out _);
    }
}