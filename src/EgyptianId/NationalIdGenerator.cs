namespace EgyptianId;

/// <summary>
/// Generates structurally valid national ID numbers for tests and demos.
/// The numbers are random. They are not issued to anyone and the check digit is arbitrary.
/// </summary>
public static class NationalIdGenerator
{
    /// <summary>Generates one number. Pass a seeded <see cref="Random"/> for repeatable output.</summary>
    /// <param name="random">Random source. Defaults to <see cref="Random.Shared"/>.</param>
    /// <param name="gender">Gender to encode. Random when null.</param>
    /// <param name="governorate">Governorate to encode. Random when null.</param>
    /// <param name="birthDate">Birth date (years 1900 to 2099). Random between 1960 and 2005 when null.</param>
    public static string Generate(Random? random = null, Gender? gender = null, Governorate? governorate = null, DateOnly? birthDate = null)
    {
        random ??= Random.Shared;

        var start = new DateOnly(1960, 1, 1);
        var span = new DateOnly(2005, 12, 31).DayNumber - start.DayNumber + 1;
        var date = birthDate ?? start.AddDays(random.Next(span));

        if (date.Year < 1900 || date.Year > 2099)
        {
            throw new ArgumentOutOfRangeException(nameof(birthDate), "Birth year must be between 1900 and 2099.");
        }

        var all = Enum.GetValues<Governorate>();
        var gov = governorate ?? all[random.Next(all.Length)];
        var isMale = (gender ?? (random.Next(2) == 0 ? Gender.Male : Gender.Female)) == Gender.Male;

        var century = date.Year >= 2000 ? 3 : 2;
        var serial = random.Next(1000);
        var genderDigit = random.Next(5) * 2 + (isMale ? 1 : 0);
        var check = random.Next(10);

        return $"{century}{date.Year % 100:D2}{date.Month:D2}{date.Day:D2}{(int)gov:D2}{serial:D3}{genderDigit}{check}";
    }
}
