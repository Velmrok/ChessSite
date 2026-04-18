using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UsersSortBy
{
    CreatedAt,
    Nickname ,
    OnlineStatus,
    LastActive,
    Rating
}