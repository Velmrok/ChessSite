using backend.DTO.Common;
using backend.Enums;

namespace backend.DTO.Games;

public record GameResponse(
    string Id,
    UserGameSummary WhitePlayer,
    UserGameSummary BlackPlayer,
    string? WinnerNickname,
    GameStatus Status,
    int Time,
    int Increment,
    List<MoveInfo> Moves,
    GameType GameType,
    int CurrentWhiteTime,
    int CurrentBlackTime,
    List<MessageInfo> Messages,
    string? DrawOfferedBy,
    string? EndGameReason

);
public record MoveInfo(string San, string Fen, int DeltaTime, int AbsoluteTime);
public record MessageInfo(string SenderNickname, string Content, DateTime Timestamp);




