using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EgyptianId;

/// <summary>
/// Offline validation of the 14-digit Egyptian national ID number.
/// Layout: C YYMMDD GG SSSS K, where C is the century digit (2 = 1900s, 3 = 2000s),
/// GG is the governorate code, the last serial digit is odd for male and even for female,
/// and K is a check digit.
/// </summary>
/// <remarks>
/// The check digit (digit 14) is not validated: no official algorithm is published.
/// This library checks structure only. It cannot tell you that a number was actually issued.
/// </remarks>
public static class NationalIdValidator
{
    private const int Length = 14;

    /// <summary>Validates a number and returns the decoded data or the reason it was rejected.</summary>
    /// <param name="input">The number. Arabic-Indic digits, spaces and hyphens are accepted.</param>
    /// <param name="today">Reference date for the future-birth check. Defaults to today (UTC).</param>
    public static NationalIdResult Validate(string? input, DateOnly? today = null)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Fail(NationalIdError.Empty);
        }

        var number = Normalize(input);

        foreach (var c in number)
        {
            if (c < '0' || c > '9')
            {
                return Fail(NationalIdError.NonDigit);
            }
        }

        if (number.Length != Length)
        {
            return Fail(NationalIdError.InvalidLength);
        }

        var century = number[0] switch
        {
            '2' => 1900,
            '3' => 2000,
            _ => 0
        };

        if (century == 0)
        {
            return Fail(NationalIdError.InvalidCentury);
        }

        var year = century + int.Parse(number.AsSpan(1, 2));
        var month = int.Parse(number.AsSpan(3, 2));
        var day = int.Parse(number.AsSpan(5, 2));

        if (month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
        {
            return Fail(NationalIdError.InvalidBirthDate);
        }

        var birthDate = new DateOnly(year, month, day);
        var reference = today ?? DateOnly.FromDateTime(DateTime.UtcNow);

        if (birthDate > reference)
        {
            return Fail(NationalIdError.FutureBirthDate);
        }

        var governorate = (Governorate)int.Parse(number.AsSpan(7, 2));

        if (!Enum.IsDefined(governorate))
        {
            return Fail(NationalIdError.InvalidGovernorate);
        }

        var gender = (number[12] - '0') % 2 == 1 ? Gender.Male : Gender.Female;

        return new NationalIdResult(NationalIdError.None, new NationalIdInfo(number, birthDate, gender, governorate));
    }

    /// <summary>True when the number passes every structural check.</summary>
    public static bool IsValid(string? input, DateOnly? today = null) => Validate(input, today).IsValid;

    /// <summary>Tries to decode a number.</summary>
    public static bool TryParse(string? input, [NotNullWhen(true)] out NationalIdInfo? info, DateOnly? today = null)
    {
        info = Validate(input, today).Info;
        return info is not null;
    }

    private static NationalIdResult Fail(NationalIdError error) => new(error, null);

    private static string Normalize(string input)
    {
        var sb = new StringBuilder(input.Length);

        foreach (var c in input.Trim())
        {
            if (c >= '\u0660' && c <= '\u0669')
            {
                sb.Append((char)('0' + (c - '\u0660')));
            }
            else if (c >= '\u06F0' && c <= '\u06F9')
            {
                sb.Append((char)('0' + (c - '\u06F0')));
            }
            else if (c is ' ' or '-')
            {
                continue;
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
