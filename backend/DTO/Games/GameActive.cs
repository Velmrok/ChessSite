
using backend.Enums;

namespace backend.DTO.Games;
public class GameActive
{
    public string Id { get; set; } = default!;

    public string WhitePlayerId { get; set; } = default!;
    public string WhitePlayerNickname { get; set; } = default!;
    public string WhitePlayerProfilePictureUrl { get; set; } = default!;
    public int WhitePlayerRating { get; set; }
    public int CurrentWhiteTime { get; set; }

    public string BlackPlayerId { get; set; } = default!;
    public string BlackPlayerNickname { get; set; } = default!;
    public string BlackPlayerProfilePictureUrl { get; set; } = default!;
    public int BlackPlayerRating { get; set; }
    public int CurrentBlackTime { get; set; }

    public int Time { get; set; }
    public int Increment { get; set; }
    public GameType GameType { get; set; }
    public string? DrawOfferedBy { get; set; }
}



