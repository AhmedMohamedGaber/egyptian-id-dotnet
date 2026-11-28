using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EgyptianId.AspNetCore;

/// <summary>
/// Binds a form, query or route value straight to a decoded <see cref="NationalIdInfo"/>.
/// Empty values bind to null so <c>[Required]</c> keeps its usual meaning.
/// Invalid values add a model-state error naming the exact reason, and with
/// <c>[ApiController]</c> the client automatically gets a 400 response.
/// </summary>
public sealed class NationalIdInfoModelBinder : IModelBinder
{
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        var result = NationalIdValidator.Validate(value);
        if (result.Info is { } info)
        {
            bindingContext.Result = ModelBindingResult.Success(info);
        }
        else
        {
            var name = bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelName;
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                $"The {name} field is not a valid Egyptian national ID number ({Describe(result.Error)}).");
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }

    private static string Describe(NationalIdError error) => error switch
    {
        NationalIdError.NonDigit => "digits only, please",
        NationalIdError.InvalidLength => "exactly 14 digits",
        NationalIdError.InvalidCentury => "the first digit must be 2 or 3",
        NationalIdError.InvalidBirthDate => "the birth date is not a real calendar date",
        NationalIdError.FutureBirthDate => "the birth date is in the future",
        NationalIdError.InvalidGovernorate => "unknown governorate code",
        _ => error.ToString()
    };
}
