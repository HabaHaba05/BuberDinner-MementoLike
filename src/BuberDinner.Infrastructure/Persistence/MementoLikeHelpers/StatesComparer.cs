using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

public static class StatesComparer
{
    public record ComparerResult(ActionType ActionType, JObject Changes, List<KeyValuePair<string, object>> Keys, JObject? CurrentState, JObject? PreviousState);

    public static Dictionary<string, List<ComparerResult>> Handle(
        Dictionary<string, object?> oldState,
        Dictionary<string, object?> newState,
        string tableName) =>
        Handle(JObject.FromObject(oldState), JObject.FromObject(newState), tableName);

    public static Dictionary<string, List<ComparerResult>> Handle(JObject oldState, JObject newState, string tableName)
    {
        var result = new Dictionary<string, List<ComparerResult>>();

        var oldStateTableRecords = StateToTableStructureConverter.Handle(oldState, tableName);
        var newStateTableRecords = StateToTableStructureConverter.Handle(newState, tableName);

        foreach (var (table, newStateRecords) in newStateTableRecords)
        {
            if (oldStateTableRecords.TryGetValue(table, out var oldTableRecords))
            {
                var keys = oldTableRecords.Keys;
                foreach (var newStateRecord in newStateRecords.States)
                {
                    var recordInOldState = oldTableRecords.States
                        .SingleOrDefault(x => keys.All(key => ValuesEquals(x[key]!, newStateRecord[key]!)));

                    if (recordInOldState is null)
                    {
                        result.TryAdd(table, []);
                        result[table].Add(new ComparerResult(
                            ActionType.INSERT,
                            newStateRecord,
                            newStateRecords.Keys.Select(k => new KeyValuePair<string, object>(k, ((JValue)newStateRecord[k]!).Value!)).ToList(),
                            newStateRecord,
                            null));
                    }
                    else
                    {
                        var changedProperties = new JObject();
                        foreach (var newStateProperty in newStateRecord)
                        {
                            if (keys.Contains(newStateProperty.Key))
                            {
                                continue;
                            }

                            if (!recordInOldState.ContainsKey(newStateProperty.Key))
                            {
                                throw new InvalidOperationException("Table states must contain data for same columns");
                            }

                            if (!ValuesEquals(recordInOldState[newStateProperty.Key]!, newStateProperty.Value!))
                            {
                                changedProperties.Add(newStateProperty.Key, newStateProperty.Value);
                            }
                        }

                        if (changedProperties.Count > 0)
                        {
                            if (result.ContainsKey(table))
                            {
                                result[table].Add(new ComparerResult(
                                    ActionType.UPDATE,
                                    changedProperties,
                                    keys.Select(k => new KeyValuePair<string, object>(k, ((JValue)newStateRecord[k]!).Value!)).ToList(),
                                    newStateRecord,
                                    recordInOldState));
                            }
                            else
                            {
                                result.Add(table, [new ComparerResult(
                                    ActionType.UPDATE,
                                    changedProperties,
                                    keys.Select(k => new KeyValuePair<string, object>(k, ((JValue)newStateRecord[k]!).Value!)).ToList(),
                                    newStateRecord,
                                    recordInOldState)]);
                            }
                        }
                    }
                }
            }
            else
            {
                result.TryAdd(table, []);
                newStateRecords.States.ForEach(x => result[table].Add(new ComparerResult(
                    ActionType.INSERT,
                    x,
                    newStateRecords.Keys.Select(k => new KeyValuePair<string, object>(k, ((JValue)x[k]!).Value!)).ToList(),
                    x,
                    null)));
            }
        }

        foreach (var (table, oldStateRecords) in oldStateTableRecords)
        {
            if (newStateTableRecords.TryGetValue(table, out var newStateRecords))
            {
                var keys = newStateRecords.Keys;
                foreach (var oldStateRecord in oldStateRecords.States)
                {
                    var recordInNewState = newStateRecords.States.SingleOrDefault(
                        x => keys.All(key => ValuesEquals(x[key]!, oldStateRecord[key]!)));

                    if (recordInNewState is null)
                    {
                        result.TryAdd(table, []);
                        result[table].Add(new ComparerResult(
                            ActionType.REMOVE,
                            oldStateRecord,
                            keys.Select(k => new KeyValuePair<string, object>(k, ((JValue)oldStateRecord[k]!).Value!)).ToList(),
                            null,
                            oldStateRecord));
                    }
                }
            }
            else
            {
                result.TryAdd(table, []);
                oldStateRecords.States.ForEach(x => result[table].Add(new ComparerResult(
                    ActionType.REMOVE,
                    x,
                    oldStateRecords.Keys.Select(k => new KeyValuePair<string, object>(k, ((JValue)x[k]!).Value!)).ToList(),
                    null,
                    x)));
            }
        }

        return result;
    }

    private static bool ValuesEquals(JToken obj1, JToken obj2) => JsonConvert.SerializeObject(obj1) == JsonConvert.SerializeObject(obj2);
}