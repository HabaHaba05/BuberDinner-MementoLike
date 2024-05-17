using Newtonsoft.Json.Linq;

namespace BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
public static class StateToTableStructureConverter
{
    public record ConverterResult(List<string> Keys, List<JObject> States);

    public static Dictionary<string, ConverterResult> Handle(
        Dictionary<string, object?> memento,
        string tableName) => Handle(JObject.FromObject(memento), tableName);

    public static Dictionary<string, ConverterResult> Handle(
        JObject memento,
        string tableName)
    {
        var result = new Dictionary<string, ConverterResult>();

        var keys = new List<string>();
        var tableRecordValues = new JObject();

        var objectOrArrayKeys = new List<string>();

        foreach (var property in memento.Properties())
        {
            var name = property.Name;
            var value = property.Value;

            if (name == "Keys")
            {
                foreach (var arrayItem in (JArray)value)
                {
                    keys.Add((string)arrayItem!);
                }

                continue;
            }

            if (value.Type is JTokenType.Array or JTokenType.Object)
            {
                objectOrArrayKeys.Add(name);
            }
            else
            {
                tableRecordValues.Add(name, value);
            }
        }

        if (tableRecordValues.HasValues)
        {
            if (result.ContainsKey(tableName))
            {
                result[tableName].States.Add(tableRecordValues);
            }
            else
            {
                result.Add(tableName, new(keys, [tableRecordValues]));
            }
        }

        foreach (var key in objectOrArrayKeys)
        {
            var value = memento[key];
            if (value!.Type is JTokenType.Array)
            {
                foreach (JObject arrayItem in value)
                {
                    var parsedValues = Handle(arrayItem, key);
                    result.AppendDictionary(parsedValues);
                }
            }
            else if (value.Type is JTokenType.Object)
            {
                var parsedValues = Handle((JObject)value, key);
                result.AppendDictionary(parsedValues);
            }
        }

        return result;
    }

    private static void AppendDictionary(
        this Dictionary<string, ConverterResult> dest,
        Dictionary<string, ConverterResult> src)
    {
        foreach (var kvp in src)
        {
            if (!dest.ContainsKey(kvp.Key))
            {
                dest[kvp.Key] = kvp.Value;
            }
            else
            {
                dest[kvp.Key].States.AddRange(kvp.Value.States);
            }
        }
    }
}