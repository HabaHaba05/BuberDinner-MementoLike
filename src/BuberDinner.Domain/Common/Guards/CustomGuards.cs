using System.Runtime.CompilerServices;

using Ardalis.GuardClauses;

namespace BuberDinner.Domain.Common.Guards
{
    public static class CustomGuards
    {
        public static void DoesNotExist<T>(
            this IGuardClause guardClause,
            List<T> input,
            T item,
            [CallerArgumentExpression("input")] string? parameterName = null)
        {
            if (input.Contains(item))
            {
                throw new ArgumentException("List should not contain item!", parameterName);
            }
        }

        public static void Contains<T>(
            this IGuardClause guardClause,
            List<T> input,
            T item,
            [CallerArgumentExpression("input")] string? parameterName = null)
        {
            if (!input.Contains(item))
            {
                throw new ArgumentException("List should contain item!", parameterName);
            }
        }

        public static void OneOfTheListContains<T>(
            this IGuardClause guardClause,
            List<T> input,
            List<T> input2,
            T item)
        {
            if (input.Contains(item) && input2.Contains(item))
            {
                throw new ArgumentException("Only one of the lists can contain the item!");
            }
            else if (!input.Contains(item) && !input2.Contains(item))
            {
                throw new ArgumentException("At least one of the lists must contain the item!");
            }
        }
    }
}