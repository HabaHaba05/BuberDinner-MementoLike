using Dapper;

namespace BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Helpers;
public static class QueryAndParametersLogger
{
    public static void WriteToConsoleQueryAndParameters(string query, DynamicParameters parameters)
    {
        var parametersStr = string.Join(", ", from pn in parameters.ParameterNames select string.Format("@{0}={1}", pn, (parameters as SqlMapper.IParameterLookup)[pn]));
        Console.WriteLine(Environment.NewLine + query);
        Console.WriteLine("Parameters:" + parametersStr + Environment.NewLine);
    }
}