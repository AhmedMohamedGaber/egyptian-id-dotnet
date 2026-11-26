using System.ComponentModel.DataAnnotations;

namespace EgyptianId;

/// <summary>
/// DataAnnotations attribute for ASP.NET, Blazor and any model validation.
/// Null and empty values pass, so combine with <c>[Required]</c> when the field is mandatory.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class EgyptianNationalIdAttribute : ValidationAttribute
{
    /// <summary>Creates the attribute with the default error message.</summary>
    public EgyptianNationalIdAttribute()
        : base("The {0} field is not a valid Egyptian national ID number.")
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is string text)
        {
            return text.Length == 0 || NationalIdValidator.IsValid(text);
        }

        return false;
    }
}
