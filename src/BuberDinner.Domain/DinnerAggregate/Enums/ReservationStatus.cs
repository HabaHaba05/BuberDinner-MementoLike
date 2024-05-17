using Ardalis.SmartEnum;

namespace BuberDinner.Domain.DinnerAggregate.Enums;

public class ReservationStatus : SmartEnum<ReservationStatus>
{
    public ReservationStatus(string name, int value) : base(name, value)
    {
    }

    public static readonly ReservationStatus PendingGuestApproval = new(nameof(PendingGuestApproval), 1);
    public static readonly ReservationStatus Rejected = new(nameof(Rejected), 2);
    public static readonly ReservationStatus NoAnswer = new(nameof(NoAnswer), 3);
    public static readonly ReservationStatus Reserved = new(nameof(Reserved), 4);
    public static readonly ReservationStatus Cancelled = new(nameof(Cancelled), 5);
    public static readonly ReservationStatus Arrived = new(nameof(Arrived), 6);
    public static readonly ReservationStatus Completed = new(nameof(Completed), 7);
    public static readonly ReservationStatus NonArrival = new(nameof(NonArrival), 8);
}