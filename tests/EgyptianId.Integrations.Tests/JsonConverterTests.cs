using System.Text.Json;
using EgyptianId;

namespace EgyptianId.Integrations.Tests;

public class JsonConverterTests
{
    private const string Valid = "29902140100115"; // 14 Feb 1999, Cairo, male

    private sealed class Holder
    {
        public NationalIdInfo? NationalId { get; set; }
    }

    [Fact]
    public void Serializes_as_the_plain_14_digit_string()
    {
        var info = NationalIdValidator.Parse(Valid);

        Assert.Equal($"\"{Valid}\"", JsonSerializer.Serialize(info));
    }

    [Fact]
    public void Deserializes_a_valid_number_with_all_fields_decoded()
    {
        var info = JsonSerializer.Deserialize<NationalIdInfo>($"\"{Valid}\"");

        Assert.NotNull(info);
        Assert.Equal(Valid, info.Number);
        Assert.Equal(new DateOnly(1999, 2, 14), info.BirthDate);
        Assert.Equal(Gender.Male, info.Gender);
        Assert.Equal(Governorate.Cairo, info.BirthGovernorate);
    }

    [Fact]
    public void Round_trips_through_a_containing_model_without_any_options()
    {
        var holder = new Holder { NationalId = NationalIdValidator.Parse(Valid) };

        var json = JsonSerializer.Serialize(holder);
        var back = JsonSerializer.Deserialize<Holder>(json);

        Assert.Equal($"{{\"NationalId\":\"{Valid}\"}}", json);
        Assert.Equal(holder.NationalId, back?.NationalId);
    }

    [Fact]
    public void Accepts_arabic_indic_digits_and_stores_them_normalized()
    {
        var info = JsonSerializer.Deserialize<NationalIdInfo>("\"٢٩٩٠٢١٤٠١٠٠١١٥\"");

        Assert.Equal(Valid, info?.Number);
    }

    [Fact]
    public void Deserializes_json_null_as_null()
    {
        Assert.Null(JsonSerializer.Deserialize<NationalIdInfo>("null"));
        Assert.Null(JsonSerializer.Deserialize<Holder>("{\"NationalId\":null}")?.NationalId);
    }

    [Theory]
    [InlineData("\"2990214010011\"")]       // 13 digits
    [InlineData("\"29902140100115999\"")]   // 17 digits
    [InlineData("\"49902140100115\"")]      // bad century digit
    [InlineData("\"29902320100115\"")]      // 32nd of February
    [InlineData("\"29902149900115\"")]      // unknown governorate 99
    [InlineData("\"2990214010011x\"")]      // not a digit
    public void Rejects_an_invalid_number_with_a_named_reason(string json)
    {
        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<NationalIdInfo>(json));

        Assert.Contains("not a valid Egyptian national ID number", ex.Message);
    }

    [Fact]
    public void Rejects_a_non_string_token()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<NationalIdInfo>("29902140100115"));
    }

    [Fact]
    public void NationalIdInfo_TryParse_supports_minimal_api_binding_shape()
    {
        Assert.True(NationalIdInfo.TryParse(Valid, out var info));
        Assert.NotNull(info);
        Assert.False(NationalIdInfo.TryParse("123", out var bad));
        Assert.Null(bad);
    }
}
