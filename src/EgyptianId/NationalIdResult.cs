namespace EgyptianId;

/// <summary>Why a national ID number was rejected.</summary>
public enum NationalIdError
{
    /// <summary>No error. The number is structurally valid.</summary>
    None,

    /// <summary>The input was null, empty or whitespace.</summary>
    Empty,

    /// <summary>The input contains a character that is not a digit.</summary>
    NonDigit,

    /// <summary>The input does not have exactly 14 digits.</summary>
    InvalidLength,

    /// <summary>The first digit is not 2 (1900s) or 3 (2000s).</summary>
    InvalidCentury,

    /// <summary>Digits 2 to 7 are not a real calendar date.</summary>
    InvalidBirthDate,

    /// <summary>The birth date is after the reference date.</summary>
    FutureBirthDate,

    /// <summary>Digits 8 and 9 are not an assigned governorate code.</summary>
    InvalidGovernorate
}

/// <summary>The data encoded in a structurally valid national ID number.</summary>
/// <param name="Number">The normalized 14-digit number (ASCII digits only).</param>
/// <param name="BirthDate">Date of birth.</param>
/// <param name="Gender">Gender from digit 13.</param>
/// <param name="BirthGovernorate">Governorate of birth registration.</param>
public sealed record NationalIdInfo(string Number, DateOnly BirthDate, Gender Gender, Governorate BirthGovernorate)
{
    /// <summary>Completed years of age today (UTC).</summary>
    public int Age => AgeOn(DateOnly.FromDateTime(DateTime.UtcNow));

    /// <summary>True when the person is at least 18 on the given date (default today, UTC).</summary>
    public bool IsAdult(DateOnly? today = null) => AgeOn(today ?? DateOnly.FromDateTime(DateTime.UtcNow)) >= 18;

    /// <summary>The number with all but the last four digits hidden, safe for logs and screens.</summary>
    public string Masked => NationalIdFormatter.Mask(Number);

    /// <summary>The number grouped by field: C YYMMDD GG SSSS K.</summary>
    public string Formatted => NationalIdFormatter.Format(Number);

    /// <summary>Completed years of age on the given date.</summary>
    public int AgeOn(DateOnly date)
    {
        var age = date.Year - BirthDate.Year;
        if (date < BirthDate.AddYears(age))
        {
            age--;
        }

        return age;
    }
}

/// <summary>Outcome of validating a national ID number.</summary>
/// <param name="Error">The reason for rejection, or <see cref="NationalIdError.None"/>.</param>
/// <param name="Info">The decoded data when the number is valid, otherwise null.</param>
public sealed record NationalIdResult(NationalIdError Error, NationalIdInfo? Info)
{
    /// <summary>True when the number passed every structural check.</summary>
    public bool IsValid => Error == NationalIdError.None;
}
