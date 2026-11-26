namespace EgyptianId.Tests;

public class NationalIdValidatorTests
{
    private static readonly DateOnly Today = new(2026, 11, 26);

    [Fact]
    public void Valid_male_born_1999_in_cairo()
    {
        var result = NationalIdValidator.Validate("29902140100115", Today);

        Assert.True(result.IsValid);
        Assert.NotNull(result.Info);
        Assert.Equal(new DateOnly(1999, 2, 14), result.Info!.BirthDate);
        Assert.Equal(Gender.Male, result.Info.Gender);
        Assert.Equal(Governorate.Cairo, result.Info.BirthGovernorate);
    }

    [Fact]
    public void Valid_female_born_2005_in_giza()
    {
        var result = NationalIdValidator.Validate("30506302100227", Today);

        Assert.True(result.IsValid);
        Assert.Equal(new DateOnly(2005, 6, 30), result.Info!.BirthDate);
        Assert.Equal(Gender.Female, result.Info.Gender);
        Assert.Equal(Governorate.Giza, result.Info.BirthGovernorate);
    }

    [Fact]
    public void Arabic_indic_digits_are_accepted()
    {
        var arabic = "٢٩٩٠٢١٤٠١٠٠١١٥";

        var result = NationalIdValidator.Validate(arabic, Today);

        Assert.True(result.IsValid);
        Assert.Equal("29902140100115", result.Info!.Number);
    }

    [Fact]
    public void Spaces_and_hyphens_are_ignored()
    {
        Assert.True(NationalIdValidator.IsValid("2 990214 01 0011 5", Today));
        Assert.True(NationalIdValidator.IsValid("2-990214-01-0011-5", Today));
    }

    [Theory]
    [InlineData(null, NationalIdError.Empty)]
    [InlineData("", NationalIdError.Empty)]
    [InlineData("   ", NationalIdError.Empty)]
    [InlineData("2990214010011", NationalIdError.InvalidLength)]
    [InlineData("299021401001155", NationalIdError.InvalidLength)]
    [InlineData("2990214010011x", NationalIdError.NonDigit)]
    [InlineData("19902140100115", NationalIdError.InvalidCentury)]
    [InlineData("49902140100115", NationalIdError.InvalidCentury)]
    [InlineData("29902300100115", NationalIdError.InvalidBirthDate)]
    [InlineData("29913140100115", NationalIdError.InvalidBirthDate)]
    [InlineData("29902000100115", NationalIdError.InvalidBirthDate)]
    [InlineData("29902140500115", NationalIdError.InvalidGovernorate)]
    [InlineData("29902142000115", NationalIdError.InvalidGovernorate)]
    public void Invalid_numbers_report_the_reason(string? input, NationalIdError expected)
    {
        var result = NationalIdValidator.Validate(input, Today);

        Assert.False(result.IsValid);
        Assert.Null(result.Info);
        Assert.Equal(expected, result.Error);
    }

    [Fact]
    public void Birth_date_after_reference_date_is_rejected()
    {
        var result = NationalIdValidator.Validate("32703010100115", Today);

        Assert.Equal(NationalIdError.FutureBirthDate, result.Error);
    }

    [Fact]
    public void Leap_day_is_valid_only_in_leap_years()
    {
        Assert.True(NationalIdValidator.IsValid("30002290100115", Today));
        Assert.False(NationalIdValidator.IsValid("30102290100115", Today));
    }

    [Fact]
    public void Born_outside_republic_uses_code_88()
    {
        var result = NationalIdValidator.Validate("29902148800115", Today);

        Assert.Equal(Governorate.OutsideRepublic, result.Info!.BirthGovernorate);
    }

    [Fact]
    public void TryParse_returns_info()
    {
        Assert.True(NationalIdValidator.TryParse("29902140100115", out var info, Today));
        Assert.Equal(Governorate.Cairo, info.BirthGovernorate);
        Assert.False(NationalIdValidator.TryParse("nope", out _, Today));
    }

    [Fact]
    public void Age_counts_completed_years()
    {
        var info = NationalIdValidator.Validate("29902140100115", Today).Info!;

        Assert.Equal(26, info.AgeOn(new DateOnly(2026, 2, 13)));
        Assert.Equal(27, info.AgeOn(new DateOnly(2026, 2, 14)));
    }

    [Fact]
    public void Every_governorate_has_an_arabic_name()
    {
        foreach (var g in Enum.GetValues<Governorate>())
        {
            Assert.NotEqual(g.ToString(), g.ArabicName());
        }

        Assert.Equal("القاهرة", Governorate.Cairo.ArabicName());
    }
}
