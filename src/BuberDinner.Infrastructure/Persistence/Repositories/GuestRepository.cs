using System.Data;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.GuestAggregate;
using BuberDinner.Domain.GuestAggregate.ValueObjects;
using BuberDinner.Domain.UserAggregate.ValueObjects;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Helpers;

using Dapper;

namespace BuberDinner.Infrastructure.Persistence.Repositories;
public class GuestRepository : RepositoryBase, IGuestRepository
{
    private readonly IdentityMap _identityMap;

    public GuestRepository(
        IdentityMap map,
        IDbTransaction transaction)
        : base(transaction)
    {
        _identityMap = map;
    }

    public async Task AddAsync(Guest guest)
    {
        var queriesWithParams = SqlQueryBuilder.Insert(guest.GetState(), Guest.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }

    public async Task<Guest?> GetByIdAsync(GuestId guestId)
    {
        return (await GetByIdsAsync([guestId])).FirstOrDefault();
    }

    public async Task<List<Guest>> GetByIdsAsync(List<GuestId> guestIds)
    {
        var query = @$"
            SELECT * FROM Guests WHERE Id IN (@GuestIds);
	        SELECT * FROM GuestMenuReviewIds WHERE GuestId IN (@GuestIds);
	        SELECT * FROM GuestBillIds WHERE GuestId IN (@GuestIds);
	        SELECT * FROM GuestPendingReservationIds WHERE GuestId IN (@GuestIds);
	        SELECT * FROM GuestUpcomingReservationIds WHERE GuestId IN (@GuestIds);
	        SELECT * FROM GuestOngoingReservationIds WHERE GuestId IN (@GuestIds);
	        SELECT * FROM GuestPastReservationIds WHERE GuestId IN (@GuestIds);
	        SELECT * FROM GuestRatings WHERE GuestId IN (@GuestIds);
        ";

        var parameters = new DynamicParameters();
        parameters.Add("GuestIds", string.Join(", ", guestIds.Select(x => x.Value)));
        QueryAndParametersLogger.WriteToConsoleQueryAndParameters(query, parameters);

        var queryResult = await Connection.QueryMultipleAsync(query, parameters, Transaction);

        var guests = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var menuReviewIds = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var billIds = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var pendingReservationIds = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var upcomingReservationIds = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var ongoingReservationIds = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var pastReservationIds = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();
        var ratings = queryResult.Read().ToList().Select(x => ((IDictionary<string, object?>)x).ToDictionary()).ToList();

        foreach (var guest in guests)
        {
            var guestReviewIds = menuReviewIds.Where(x => x["GuestId"]!.ToString() == guest["Id"]!.ToString()).Select(x => x["MenuReviewId"]!.ToString()).ToList();
            guest.Add("GuestMenuReviewIds", guestReviewIds);
            var guestBillIds = billIds.Where(x => x["GuestId"]!.ToString() == guest["Id"]!.ToString()).Select(x => x["BillId"]!.ToString()).ToList();
            guest.Add("GuestBillIds", guestBillIds);
            var guestPendingReservationIds = pendingReservationIds.Where(x => x["GuestId"]!.ToString() == guest["Id"]!.ToString()).Select(x => x["ReservationId"]!.ToString()).ToList();
            guest.Add("GuestPendingReservationIds", guestPendingReservationIds);
            var guestUpcomingReservationIds = upcomingReservationIds.Where(x => x["GuestId"]!.ToString() == guest["Id"]!.ToString()).Select(x => x["ReservationId"]!.ToString()).ToList();
            guest.Add("GuestUpcomingReservationIds", guestUpcomingReservationIds);
            var guestOngoingReservationIds = ongoingReservationIds.Where(x => x["GuestId"]!.ToString() == guest["Id"]!.ToString()).Select(x => x["ReservationId"]!.ToString()).ToList();
            guest.Add("GuestOngoingReservationIds", guestOngoingReservationIds);
            var guestPastReservationIds = pastReservationIds.Where(x => x["GuestId"]!.ToString() == guest["Id"]!.ToString()).Select(x => x["ReservationId"]!.ToString()).ToList();
            guest.Add("GuestPastReservationIds", guestPastReservationIds);
            var guestRatings = ratings.Where(x => x["GuestId"]!.ToString() == guest["Id"]!.ToString()).ToList();
            guest.Add("GuestRatings", guestRatings);
        }

        return guests.Select(Guest.FromState).ToList();
    }

    public async Task<GuestId?> GetGuestIdOfUserAsync(UserId userId)
    {
        var queryWithParams = SqlQueryBuilder.Select(Guest.TableName, "Id", [new("UserId", userId.Value)]);
        var queryResult = ((IDictionary<string, object?>?)await Connection
            .QuerySingleOrDefaultAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction))?
            .ToDictionary(x => x.Key, x => x.Value);

        return queryResult is null ? null : GuestId.Create((Guid)queryResult["Id"]!);
    }

    public async Task UpdateAsync(Guest guest)
    {
        var currentState = guest.GetState();
        var previousState = _identityMap.FindByKeys(guest)!.OriginalState!;
        var queriesWithParams = SqlQueryBuilder.Update(previousState, currentState, Guest.TableName);
        foreach (var queryWithParams in queriesWithParams)
        {
            await Connection.ExecuteAsync(queryWithParams.Query, queryWithParams.Parameters, transaction: Transaction);
        }
    }

    public async Task UpdateRangeAsync(List<Guest> guests)
    {
        await Task.CompletedTask;
        guests.ForEach(async x => await UpdateAsync(x));
    }
}