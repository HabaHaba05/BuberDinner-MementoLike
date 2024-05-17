namespace BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Extensions;

public static class DictionaryExtensions
{
    public static string GetKeysSeparatedByComma(this Dictionary<string, object?> dictionary)
        => string.Join(", ", dictionary.Keys);
    public static string GetKeysSeparatedByCommaAndEachKeyHasAtSignAsPrefix(this Dictionary<string, object?> dictionary, string prefix = "")
        => string.Join(", ", dictionary.Keys.Select(x => "@" + prefix + x));
    public static string CreateParametersForUpdateOrWhereClauses(this Dictionary<string, object?> dictionary)
        => string.Join(", ", dictionary.Keys.Select(x => x + "=@" + x));
    public static string CreateParametersForUpdateOrWhereClauses(this List<KeyValuePair<string, object?>> dictionary)
        => string.Join(", ", dictionary.Select(x => x.Key + "=@" + x.Key));
    public static string CreateParametersForWhereClause(this List<KeyValuePair<string, object?>> dictionary)
    {
        Func<string, int, string> createKeys = (key, count) =>
        {
            var keys = string.Empty;
            for (var i = 0; i < count; i++)
            {
                keys += $"{i}_{key}";
            }

            return keys;
        };

        return string.Join(", ", dictionary.Select(
            x => x.Value?.GetType().IsArray ?? false
             ? x.Key + "IN (" + createKeys(x.Key, ((Array)x.Value!).Length) + ")"
             : x.Key + "=@" + x.Key));
    }
}