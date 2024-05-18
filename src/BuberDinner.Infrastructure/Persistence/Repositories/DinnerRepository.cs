using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.DinnerAggregate;
using BuberDinner.Domain.DinnerAggregate.ValueObjects;
using BuberDinner.Domain.HostAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Helpers;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;
public class DinnerRepository : RepositoryBase, IDinnerRepository
{
    private readonly IdentityMap _identityMap;

    public DinnerRepository(
        IdentityMap map,
        IDbTransaction transaction)
        : base(transaction)
    {
        _identityMap = map;
    }

    public async Task AddAsync(Dinner dinner)
    {
        var queriesWithParams = SqlQueryBuilder.Insert(dinner.GetState(), Dinner.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }

    public async Task<Dinner?> GetByIdAsync(DinnerId dinnerId)
    {
        var query = @$"
            SELECT * FROM Dinners WHERE Id = @DinnerId;
	        SELECT * FROM DinnerReservations WHERE DinnerId = @DinnerId;
        ";
        var parameters = new DynamicParameters(new { DinnerId = dinnerId.Value });
        parameters.Add("DinnerId", dinnerId.Value);
        QueryAndParametersLogger.WriteToConsoleQueryAndParameters(query, parameters);
        var queryResult = await Connection.QueryMultipleAsync(query, parameters, transaction: Transaction);
        var dinner = ((IDictionary<string, object?>)queryResult.ReadFirst()).ToDictionary(x => x.Key, x => x.Value);
        var reservations = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();

        if (dinner is not null)
        {
            dinner.Add("DinnerReservations", reservations);
            return Dinner.FromState(dinner);
        }

        return null;
    }

    public async Task<Dinner?> GetByReservationIdAsync(ReservationId reservationId)
    {
        var query = @$"
            SELECT * FROM Dinners WHERE Id = (SELECT DinnerId FROM DinnerReservations WHERE ReservationId = @ReservationId);
            SELECT * FROM DinnerReservations WHERE DinnerId = (SELECT DinnerId FROM DinnerReservations WHERE ReservationId = @ReservationId);
        ";
        var parameters = new DynamicParameters();
        parameters.Add("ReservationId", reservationId.Value);
        QueryAndParametersLogger.WriteToConsoleQueryAndParameters(query, parameters);

        var queryResult = await Connection.QueryMultipleAsync(query, parameters, transaction: Transaction);
        var dinner = ((IDictionary<string, object?>)queryResult.ReadFirst()).ToDictionary(x => x.Key, x => x.Value);
        var reservations = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();

        if (dinner is not null)
        {
            dinner.Add("DinnerReservations", reservations);
            return Dinner.FromState(dinner);
        }

        return null;
    }

    public async Task<List<Dinner>> ListAsync(HostId hostId)
    {
        var query = @$"
            SELECT * FROM Dinners WHERE HostId = @HostId;
	        SELECT * FROM DinnerReservations WHERE
                DinnerId IN (SELECT Id FROM Dinners WHERE HostId = @HostId);
        ";
        var parameters = new DynamicParameters();
        parameters.Add("HostId", hostId.Value);
        QueryAndParametersLogger.WriteToConsoleQueryAndParameters(query, parameters);

        var queryResult = await Connection.QueryMultipleAsync(query, parameters, transaction: Transaction);
        var dinners = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var reservations = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();

        foreach (var dinner in dinners)
        {
            var dinnerReservations = reservations.Where(x => x["DinnerId"]!.ToString() == dinner["Id"]!.ToString()).ToList();
            dinner.Add("DinnerReservations", dinnerReservations);
        }

        return dinners.Select(Dinner.FromState).ToList();
    }

    public async Task UpdateAsync(Dinner dinner)
    {
        var currentState = dinner.GetState();
        var previousState = _identityMap.FindByKeys(dinner)!.OriginalState!;
        var queriesWithParams = SqlQueryBuilder.Update(previousState, currentState, Dinner.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }
}