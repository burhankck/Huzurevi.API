namespace Huzurevi.Application.Common;

public static class CokluSecim
{
    public static List<string> Liste(string? ham) =>
        string.IsNullOrWhiteSpace(ham)
            ? []
            : ham.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    public static string? Metin(IEnumerable<string>? degerler)
    {
        var liste = (degerler ?? [])
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        return liste.Count == 0 ? null : string.Join(",", liste);
    }
}
