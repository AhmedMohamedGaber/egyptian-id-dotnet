using EgyptianId;
using FluentValidation;

namespace EgyptianId.Integrations.Tests;

public class FluentValidationTests
{
    private const string Valid = "29902140100115";

    private sealed class Person
    {
        public string? NationalId { get; set; }
    }

    private sealed class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator(bool allowNullOrEmpty = false)
        {
            RuleFor(x => x.NationalId).IsValidEgyptianNationalId(allowNullOrEmpty);
        }
    }

    [Fact]
    public void A_valid_number_passes()
    {
        var result = new PersonValidator().Validate(new Person { NationalId = Valid });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Arabic_indic_digits_pass()
    {
        var result = new PersonValidator().Validate(new Person { NationalId = "٢٩٩٠٢١٤٠١٠٠١١٥" });

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("2990214010011", "InvalidLength", "exactly 14 digits")]
    [InlineData("49902140100115", "InvalidCentury", "must start with 2")]
    [InlineData("29902320100115", "InvalidBirthDate", "real birth date")]
    [InlineData("29902149900115", "InvalidGovernorate", "unknown governorate code")]
    [InlineData("2990214010011x", "NonDigit", "digits only")]
    public void An_invalid_number_fails_with_a_precise_message_and_error_code(
        string number, string expectedCode, string expectedMessagePart)
    {
        var result = new PersonValidator().Validate(new Person { NationalId = number });

        var failure = Assert.Single(result.Errors);
        Assert.Equal(expectedCode, failure.ErrorCode);
        Assert.Equal(nameof(Person.NationalId), failure.PropertyName);
        Assert.DoesNotContain("{PropertyName}", failure.ErrorMessage);
        Assert.Contains(expectedMessagePart, failure.ErrorMessage);
    }

    [Fact]
    public void Null_and_empty_fail_by_default()
    {
        var validator = new PersonValidator();

        Assert.False(validator.Validate(new Person { NationalId = null }).IsValid);
        Assert.False(validator.Validate(new Person { NationalId = "" }).IsValid);
        Assert.False(validator.Validate(new Person { NationalId = "   " }).IsValid);
    }

    [Fact]
    public void Null_and_empty_pass_when_allowed()
    {
        var validator = new PersonValidator(allowNullOrEmpty: true);

        Assert.True(validator.Validate(new Person { NationalId = null }).IsValid);
        Assert.True(validator.Validate(new Person { NationalId = "" }).IsValid);
        Assert.False(validator.Validate(new Person { NationalId = "123" }).IsValid);
    }

    [Fact]
    public void The_display_name_is_used_in_the_message()
    {
        var validator = new InlineValidator<Person>();
        validator.RuleFor(x => x.NationalId)
            .NotEmpty().WithName("الرقم القومي")
            .IsValidEgyptianNationalId();

        var result = validator.Validate(new Person { NationalId = "123" });

        Assert.Contains("الرقم القومي", Assert.Single(result.Errors).ErrorMessage);
    }

    [Fact]
    public void A_future_birth_date_is_reported()
    {
        // 2099-01-01, Cairo
        var result = new PersonValidator().Validate(new Person { NationalId = "39901010100115" });

        var failure = Assert.Single(result.Errors);
        Assert.Equal("FutureBirthDate", failure.ErrorCode);
    }
}
