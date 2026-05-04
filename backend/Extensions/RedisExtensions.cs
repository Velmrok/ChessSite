using StackExchange.Redis;

namespace backend.Extensions
{
    public static class RedisExtensions
    {
        public static HashEntry[] ToHashEntries(this object obj)
        {
            var properties = obj.GetType().GetProperties();
            return properties
                .Where(p => p.GetValue(obj) != null)
                .Select(p => new HashEntry(
                    p.Name.ToLower(),
                    p.GetValue(obj)!.ToString()
                ))
                .ToArray();
        }

        public static T FromHashEntries<T>(this HashEntry[] entries) where T : new()
        {
            var obj = new T();
            var dict = entries.ToDictionary(
                e => e.Name.ToString().ToLower(),
                e => e.Value.ToString()
            );

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!dict.TryGetValue(prop.Name.ToLower(), out var value))
                    continue;

                if (prop.PropertyType.IsEnum)
                    prop.SetValue(obj, Enum.Parse(prop.PropertyType, value));
                else if (prop.PropertyType == typeof(Guid))
                    prop.SetValue(obj, Guid.Parse(value));
                else
                    prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
            }
            return obj;
        }
    }
}