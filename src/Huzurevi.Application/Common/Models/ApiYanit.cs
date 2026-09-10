using System.Text.Json.Serialization;

namespace Huzurevi.Application.Common.Models;

public class ApiYanit<T>
{
    [JsonPropertyName("success")]
    public bool Basarili { get; set; }

    [JsonPropertyName("message")]
    public string? Mesaj { get; set; }

    [JsonPropertyName("data")]
    public T? Veri { get; set; }

    [JsonPropertyName("errors")]
    public List<string>? Hatalar { get; set; }

    public static ApiYanit<T> BasariliSonuc(T veri, string? mesaj = null) =>
        new() { Basarili = true, Veri = veri, Mesaj = mesaj };

    public static ApiYanit<T> Basarisiz(string mesaj, List<string>? hatalar = null) =>
        new() { Basarili = false, Mesaj = mesaj, Hatalar = hatalar };
}
