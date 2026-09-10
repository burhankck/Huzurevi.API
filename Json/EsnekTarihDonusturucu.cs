using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Huzurevi.API.Json;

public sealed class EsnekTarihDonusturucu : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var metin = reader.GetString();
            if (string.IsNullOrWhiteSpace(metin))
            {
                throw new JsonException("Tarih boş olamaz.");
            }

            if (DateTime.TryParse(metin, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var tarih)
                || DateTime.TryParseExact(metin, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih)
                || DateTime.TryParse(metin, new CultureInfo("tr-TR"), DateTimeStyles.None, out tarih))
            {
                return tarih.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(tarih, DateTimeKind.Utc)
                    : tarih.ToUniversalTime();
            }

            throw new JsonException("Tarih formatı geçersiz.");
        }

        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Tarih boş olamaz.");
        }

        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("O"));
    }
}

public sealed class EsnekBosOlabilirTarihDonusturucu : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var metin = reader.GetString();
            if (string.IsNullOrWhiteSpace(metin))
            {
                return null;
            }

            if (DateTime.TryParse(metin, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var tarih)
                || DateTime.TryParseExact(metin, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih)
                || DateTime.TryParse(metin, new CultureInfo("tr-TR"), DateTimeStyles.None, out tarih))
            {
                return tarih.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(tarih, DateTimeKind.Utc)
                    : tarih.ToUniversalTime();
            }

            throw new JsonException("Tarih formatı geçersiz.");
        }

        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToUniversalTime().ToString("O"));
    }
}
