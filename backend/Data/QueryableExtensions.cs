using System.Linq.Expressions;

public static class QueryableExtensions
{
    public static IQueryable<T> OrderByField<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector,
        bool descending) =>
        descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
}