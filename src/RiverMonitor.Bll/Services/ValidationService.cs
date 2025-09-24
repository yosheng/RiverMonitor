using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RiverMonitor.Bll.Services;

public interface IValidationService
{
    /// <summary>
    /// 驗證指定的 DTO 物件，如果驗證失敗則拋出 BllValidationException。
    /// </summary>
    /// <typeparam name="T">要驗證的物件類型。</typeparam>
    /// <param name="instance">要驗證的物件實例。</param>
    /// <param name="cancellationToken">CancellationToken</param>
    Task ValidateAndThrowAsync<T>(T instance, CancellationToken cancellationToken = default);
    
    Task<FluentValidation.Results.ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default);
}

public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ValidateAndThrowAsync<T>(T instance, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(instance, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException($"DTO of type {typeof(T).Name} failed validation: {errorMessages}");
        }
    }

    public async Task<FluentValidation.Results.ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance), "Validation target cannot be null.");
        }

        var validator = _serviceProvider.GetService<IValidator<T>>();

        if (validator == null)
        {
            // 或者選擇靜默處理，視您的業務需求而定
            throw new InvalidOperationException($"No validator registered for type {typeof(T).FullName}.");
        }
        
        return await validator.ValidateAsync(instance, cancellationToken);    
    }
}