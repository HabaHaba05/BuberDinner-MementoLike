using Ardalis.SmartEnum;

namespace BuberDinner.Domain.BillAggregate.Enums;

public class BillStatus : SmartEnum<BillStatus>
{
    public BillStatus(string name, int value) : base(name, value)
    {
    }

    public static readonly BillStatus Unpaid = new(nameof(Unpaid), 1);
    public static readonly BillStatus Paid = new(nameof(Paid), 2);
}