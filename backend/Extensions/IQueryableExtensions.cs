using System.Linq.Expressions;

namespace backend.Extensions;
public static class IQueryableExtensions
{
    public static IQueryable<T> SortBy<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector,
        bool descending)
    {
        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}