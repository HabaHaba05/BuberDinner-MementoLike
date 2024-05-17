using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Extensions;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

public static class SqlQueryBuilder
{
    public record SqlQueryBuilderResult(string Query, DynamicParameters Parameters);

    public static List<SqlQueryBuilderResult> Update(Dictionary<string, object?> oldState, Dictionary<string, object?> newState, string rootTableName)
    {
        var statesComparerResults = StatesComparer.Handle(oldState, newState, rootTableName);
        var result = new List<SqlQueryBuilderResult>();
        foreach (var (tableName, comparerResults) in statesComparerResults)
        {
            foreach (var comparerResult in comparerResults)
            {
                var actionType = comparerResult.ActionType;
                var keys = comparerResult.Keys;
                var state = comparerResult.Changes.ToObject<Dictionary<string, object?>>()!;

                var query = string.Empty;
                var parameters = new DynamicParameters();

                if (actionType == ActionType.REMOVE)
                {
                    query = $"DELETE FROM {tableName} WHERE {keys!.CreateParametersForUpdateOrWhereClauses().Replace(",", " AND ")};";
                    keys.ForEach(k => parameters.Add(k.Key, k.Value));
                }
                else if (actionType == ActionType.INSERT)
                {
                    query =
                        $"INSERT INTO {tableName} ({state.GetKeysSeparatedByComma()}) " +
                        $"VALUES ({state.GetKeysSeparatedByCommaAndEachKeyHasAtSignAsPrefix()});";
                    state.ToList().ForEach(s => parameters.Add(s.Key, s.Value));
                }
                else if (actionType == ActionType.UPDATE)
                {
                    query =
                        $"UPDATE {tableName} " +
                        $"SET {state.CreateParametersForUpdateOrWhereClauses()} " +
                        $"WHERE ({keys!.CreateParametersForUpdateOrWhereClauses().Replace(",", " AND ")});";
                    keys.ForEach(k => parameters.Add(k.Key, k.Value));
                    state.Where(x => x.Key != "Keys").ToList().ForEach(s => parameters.Add(s.Key, s.Value));
                }

                var parametersStr = string.Join(", ", from pn in parameters.ParameterNames select string.Format("@{0}={1}", pn, (parameters as SqlMapper.IParameterLookup)[pn]));
                Console.WriteLine(Environment.NewLine + query);
                Console.WriteLine("Parameters:" + parametersStr + Environment.NewLine);

                result.Add(new(query, parameters));
            }
        }

        return result;
    }

    public static List<SqlQueryBuilderResult> Insert(Dictionary<string, object?> state, string rootTableName)
    {
        var tablesData = StateToTableStructureConverter.Handle(state, rootTableName);
        var result = new List<SqlQueryBuilderResult>();
        foreach (var (tableName, tableData) in tablesData)
        {
            var query = string.Empty;
            var parameters = new DynamicParameters();
            foreach (var (stateJson, i) in tableData.States.Select((value, i) => (value, i)))
            {
                var tableRecordData = stateJson.ToObject<Dictionary<string, object?>>()!;

                if (i == 0)
                {
                    query += $"INSERT INTO {tableName} ({tableRecordData.GetKeysSeparatedByComma()}) VALUES";
                }

                query += $"({tableRecordData.GetKeysSeparatedByCommaAndEachKeyHasAtSignAsPrefix($"{i}_")}),";
                foreach (var tableRecord in tableRecordData)
                {
                    parameters.Add($"{i}_{tableRecord.Key}", tableRecord.Value);
                }
            }

            query = query.Remove(query.Length - 1, 1) + ";";

            var parametersStr = string.Join(", ", from pn in parameters.ParameterNames select string.Format("@{0}={1}", pn, (parameters as SqlMapper.IParameterLookup)[pn]));
            Console.WriteLine(Environment.NewLine + query);
            Console.WriteLine("Parameters:" + parametersStr + Environment.NewLine);
            result.Add(new(query, parameters));
        }

        return result;
    }

    public static SqlQueryBuilderResult Select(string tableName, string selectValues, List<KeyValuePair<string, object?>> whereClauses)
    {
        var query = $"SELECT {selectValues} FROM {tableName} WHERE {whereClauses.CreateParametersForWhereClause().Replace(",", " AND ")};";
        var parameters = new DynamicParameters();

        whereClauses.ForEach(s =>
        {
            if (s.Value?.GetType()?.IsArray ?? false)
            {
                var arr = (Array)s.Value;
                for (var i = 0; i < arr.Length; i++)
                {
                    parameters.Add($"{i}_{s.Key}", arr.GetValue(i));
                }
            }
            else
            {
                parameters.Add(s.Key, s.Value);
            }
        });

        var parametersStr = string.Join(", ", from pn in parameters.ParameterNames select string.Format("@{0}={1}", pn, (parameters as SqlMapper.IParameterLookup)[pn]));
        Console.WriteLine(Environment.NewLine + query);
        Console.WriteLine("Parameters:" + parametersStr + Environment.NewLine);

        return new(query, parameters);
    }
}