using System.ComponentModel.DataAnnotations;

namespace EgyptianId.Tests;

public class FeatureTests
{
    private const string Sample = "29902140100115";
    private const string SampleArabic = "٢٩٩٠٢١٤٠١٠٠١١٥";
    private static readonly DateOnly Today = new(2026, 11, 26);

    [Fact]
    public void Normalize_converts_arabic_indic_digits()
    {
        Assert.Equal(Sample, NationalIdValidator.Normalize(SampleArabic));
    }

    [Fact]
    public void Normalize_converts_persian_digits()
    {
        Assert.Equal("1234567890", NationalIdValidator.Normalize("۱۲۳۴۵۶۷۸۹۰"));
    }

    [Fact]
    public void Normalize_handles_null()
    {
        Assert.Equal(string.Empty, NationalIdValidator.Normalize(null));
    }

    [Fact]
    public void Bidi_marks_and_nbsp_from_pasted_arabic_text_are_ignored()
    {
        var pasted = "\u200F" + Sample + "\u200E";
        Assert.True(NationalIdValidator.IsValid(pasted, Today));
        Assert.True(NationalIdValidator.IsValid("2990214\u00A00100115", Today));
    }

    [Fact]
    public void Spaces_and_hyphens_are_ignored()
    {
        Assert.True(NationalIdValidator.IsValid("2 990214 01 0011 5", Today));
        Assert.True(NationalIdValidator.IsValid("2-990214-01-0011-5", Today));
    }

    [Fact]
    public void Parse_returns_info_for_valid_number()
    {
        var info = NationalIdValidator.Parse(Sample, Today);
        Assert.Equal(new DateOnly(1999, 2, 14), info.BirthDate);
    }

    [Fact]
    public void Parse_throws_for_invalid_number()
    {
        Assert.Throws<FormatException>(() => NationalIdValidator.Parse("123", Today));
    }

    [Fact]
    public void TryParse_works()
    {
        Assert.True(NationalIdValidator.TryParse(Sample, out var info, Today));
        Assert.Equal(Gender.Male, info!.Gender);
        Assert.False(NationalIdValidator.TryParse("nope", out var none, Today));
        Assert.Null(none);
    }

    [Theory]
    [InlineData(2026, 2, 13, 26)]
    [InlineData(2026, 2, 14, 27)]
    [InlineData(1999, 2, 14, 0)]
    [InlineData(2026, 11, 26, 27)]
    public void AgeOn_counts_completed_years(int y, int m, int d, int expected)
    {
        var info = NationalIdValidator.Parse(Sample, Today);
        Assert.Equal(expected, info.AgeOn(new DateOnly(y, m, d)));
    }

    [Fact]
    public void IsAdult_flips_on_the_eighteenth_birthday()
    {
        var info = NationalIdValidator.Parse("30506302100227", Today);
        Assert.False(info.IsAdult(new DateOnly(2023, 6, 29)));
        Assert.True(info.IsAdult(new DateOnly(2023, 6, 30)));
    }

    [Fact]
    public void Age_is_not_negative()
    {
        Assert.True(NationalIdValidator.Parse(Sample, Today).Age >= 26);
    }

    [Fact]
    public void Mask_hides_all_but_last_four()
    {
        Assert.Equal("**********0115", NationalIdFormatter.Mask(Sample));
        Assert.Equal("**********0115", NationalIdValidator.Parse(Sample, Today).Masked);
        Assert.Equal("##########0115", NationalIdFormatter.Mask(SampleArabic, 4, '#'));
        Assert.Equal(string.Empty, NationalIdFormatter.Mask(null));
    }

    [Fact]
    public void Format_groups_by_field()
    {
        Assert.Equal("2 990214 01 0011 5", NationalIdFormatter.Format(Sample));
        Assert.Equal("2 990214 01 0011 5", NationalIdValidator.Parse(Sample, Today).Formatted);
        Assert.Equal("123", NationalIdFormatter.Format("123"));
    }

    [Fact]
    public void ToArabicIndicDigits_round_trips()
    {
        Assert.Equal(SampleArabic, NationalIdFormatter.ToArabicIndicDigits(Sample));
        Assert.Equal(Sample, NationalIdValidator.Normalize(NationalIdFormatter.ToArabicIndicDigits(Sample)));
        Assert.Equal(string.Empty, NationalIdFormatter.ToArabicIndicDigits(null));
    }

    [Fact]
    public void EnglishName_is_readable()
    {
        Assert.Equal("Kafr El Sheikh", Governorate.KafrElSheikh.EnglishName());
        Assert.Equal("Port Said", Governorate.PortSaid.EnglishName());
        Assert.Equal("Cairo", Governorate.Cairo.EnglishName());
        Assert.Equal("Outside Egypt", Governorate.OutsideRepublic.EnglishName());
    }

    [Fact]
    public void Every_governorate_has_an_arabic_name()
    {
        foreach (var g in Enum.GetValues<Governorate>())
        {
            Assert.NotEqual(g.ToString(), g.ArabicName());
        }
    }

    [Fact]
    public void Attribute_accepts_valid_null_and_empty()
    {
        var attribute = new EgyptianNationalIdAttribute();
        Assert.True(attribute.IsValid(Sample));
        Assert.True(attribute.IsValid(SampleArabic));
        Assert.True(attribute.IsValid(null));
        Assert.True(attribute.IsValid(""));
    }

    [Fact]
    public void Attribute_rejects_bad_values()
    {
        var attribute = new EgyptianNationalIdAttribute();
        Assert.False(attribute.IsValid("123"));
        Assert.False(attribute.IsValid("19902140100115"));
        Assert.False(attribute.IsValid(12345));
    }

    private sealed class Person
    {
        [EgyptianNationalId]
        public string? NationalId { get; set; }
    }

    [Fact]
    public void Attribute_works_with_the_framework_validator()
    {
        var bad = new Person { NationalId = "123" };
        var good = new Person { NationalId = Sample };

        Assert.False(Validator.TryValidateObject(bad, new ValidationContext(bad), null, true));
        Assert.True(Validator.TryValidateObject(good, new ValidationContext(good), null, true));
    }

    [Fact]
    public void Generated_numbers_are_valid()
    {
        var random = new Random(42);
        for (var i = 0; i < 300; i++)
        {
            var number = NationalIdGenerator.Generate(random);
            Assert.True(NationalIdValidator.IsValid(number), number);
        }
    }

    [Fact]
    public void Generator_honours_requested_values()
    {
        var random = new Random(7);
        var date = new DateOnly(2001, 9, 5);

        for (var i = 0; i < 50; i++)
        {
            var female = NationalIdValidator.Parse(NationalIdGenerator.Generate(random, Gender.Female, Governorate.Giza, date));
            Assert.Equal(Gender.Female, female.Gender);
            Assert.Equal(Governorate.Giza, female.BirthGovernorate);
            Assert.Equal(date, female.BirthDate);

            var male = NationalIdValidator.Parse(NationalIdGenerator.Generate(random, Gender.Male));
            Assert.Equal(Gender.Male, male.Gender);
        }
    }

    [Fact]
    public void Generator_is_repeatable_with_a_seed()
    {
        Assert.Equal(NationalIdGenerator.Generate(new Random(1)), NationalIdGenerator.Generate(new Random(1)));
    }

    [Fact]
    public void Generator_rejects_out_of_range_years()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NationalIdGenerator.Generate(birthDate: new DateOnly(1850, 1, 1)));
    }

    [Fact]
    public void Leap_day_births_are_accepted_only_in_leap_years()
    {
        Assert.True(NationalIdValidator.IsValid("30002290100115", Today));
        Assert.False(NationalIdValidator.IsValid("29902290100115", Today));
    }
}
