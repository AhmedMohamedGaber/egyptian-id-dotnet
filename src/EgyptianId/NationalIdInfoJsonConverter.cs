using System.Text.Json;
using System.Text.Json.Serialization;

namespace EgyptianId;

/// <summary>
/// Converts <see cref="NationalIdInfo"/> to and from its 14-digit string form in JSON.
/// It is registered on <see cref="NationalIdInfo"/> itself, so <see cref="JsonSerializer"/>
/// round-trips the type with no setup and no extra package.
/// </summary>
public sealed class NationalIdInfoJsonConverter : JsonConverter<NationalIdInfo>
{
    /// <inheritdoc />
    public override NationalIdInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("An Egyptian national ID must be a JSON string containing the 14-digit number.");
        }

        var result = NationalIdValidator.Validate(reader.GetString());
        return result.Info ?? throw new JsonException($"The value is not a valid Egyptian national ID number ({result.Error}).");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, NationalIdInfo value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WriteStringValue(value.Number);
    }
}
