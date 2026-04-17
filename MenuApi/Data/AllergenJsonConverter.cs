using System.Text.Json;

namespace MenuApi.Data;

internal static class AllergenJsonConverter
{
    private static readonly JsonSerializerOptions Options = new();

    public static string ToJson(List<string> value) => JsonSerializer.Serialize(value, Options);

    public static List<string> FromJson(string value) =>
        string.IsNullOrEmpty(value)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(value, Options) ?? new List<string>();
}
