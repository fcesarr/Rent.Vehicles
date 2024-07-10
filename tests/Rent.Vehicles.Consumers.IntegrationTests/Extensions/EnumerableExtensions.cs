namespace Rent.Vehicles.Consumers.IntegrationTests.Extensions;

public static class EnumerableExtensions
{
    public static bool AllOrFalseIfEmpty<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);

        using (var enumerator = source.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return false;
            }

            do
            {
                if (!predicate(enumerator.Current))
                {
                    return false;
                }
            } while (enumerator.MoveNext());

            return true;
        }
    }
}
