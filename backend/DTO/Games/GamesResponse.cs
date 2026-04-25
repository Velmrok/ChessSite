using backend.Enums;

namespace backend.DTO.Games;

public record GamesResponse(List<GameSummary> Games, int TotalPages);

public record GameSummary(
    string Id,
    PlayerSummary WhitePlayer,
    PlayerSummary BlackPlayer,
    string? WinnerNickname,
    GameType Type,
    GameStatus Status,
    DateTime? FinishedAt,
    int Time,
    int Increment,
    List<string> Moves
);
public record PlayerSummary(
    string Nickname,
    string ProfilePictureUrl,
    int Rating
);



 