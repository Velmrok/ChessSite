using backend.DTO.Common;
using backend.Enums;

namespace backend.DTO.Games;

public record GamesResponse(List<GameSummary> Games, int TotalPages);

public record GameSummary(
    string Id,
    UserGameSummary WhitePlayer,
    UserGameSummary BlackPlayer,
    string? WinnerNickname,
    GameType Type,
    GameStatus Status,
    DateTime? FinishedAt,
    int Time,
    int Increment,
    List<string> Moves
);




