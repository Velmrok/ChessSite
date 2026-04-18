
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RatingType
{
    Rapid,
    Blitz,
    Bullet
}