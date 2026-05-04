using System.Text.Json.Serialization;

namespace backend.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GamesSortBy
{
    FinishedAt,
    Nickname,
    Time
}