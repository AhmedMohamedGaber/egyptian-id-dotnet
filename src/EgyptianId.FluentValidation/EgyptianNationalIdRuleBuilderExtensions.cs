using EgyptianId;
using FluentValidation.Results;

// Rule extensions live in the FluentValidation namespace so they light up
// the moment the package is installed, next to NotEmpty, EmailAddress and friends.
namespace FluentValidation;

/// <summary>FluentValidation rules for the Egyptian national ID number.</summary>
public static class EgyptianNationalIdRuleBuilderExtensions
{
    /// <summary>
    /// Validates that the string is a structurally valid 14-digit Egyptian national ID number.
    /// Arabic-Indic digits, spaces and hyphens are accepted and normalized first.
    /// The failure message names the exact reason and <see cref="ValidationFailure.ErrorCode"/>
    /// carries the <see cref="NationalIdError"/> name, so callers can localize or branch on it.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder for a string property.</param>
    /// <param name="allowNullOrEmpty">
    /// When true, null and empty values pass. Leave it false and pair with
    /// <c>NotEmpty()</c> when the field is mandatory, the usual FluentValidation style.
    /// </param>
    public static IRuleBuilderOptionsConditions<T, string?> IsValidEgyptianNationalId<T>(
        this IRuleBuilder<T, string?> ruleBuilder, bool allowNullOrEmpty = false)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);

        return ruleBuilder.Custom((value, context) =>
        {
            if (allowNullOrEmpty && string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var result = NationalIdValidator.Validate(value);
            if (!result.IsValid)
            {
                var message = MessageFor(result.Error).Replace("{PropertyName}", context.DisplayName, StringComparison.Ordinal);
                context.AddFailure(new ValidationFailure(context.PropertyName, message, value)
                {
                    ErrorCode = result.Error.ToString()
                });
            }
        });
    }

    private static string MessageFor(NationalIdError error) => error switch
    {
        NationalIdError.Empty => "{PropertyName} must not be empty.",
        NationalIdError.NonDigit => "{PropertyName} must contain digits only.",
        NationalIdError.InvalidLength => "{PropertyName} must be exactly 14 digits.",
        NationalIdError.InvalidCentury => "{PropertyName} must start with 2 (born in the 1900s) or 3 (born in the 2000s).",
        NationalIdError.InvalidBirthDate => "{PropertyName} does not contain a real birth date.",
        NationalIdError.FutureBirthDate => "{PropertyName} has a birth date in the future.",
        NationalIdError.InvalidGovernorate => "{PropertyName} has an unknown governorate code.",
        _ => "{PropertyName} is not a valid Egyptian national ID number."
    };
}
