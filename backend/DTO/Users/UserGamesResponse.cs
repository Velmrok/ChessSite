using backend.DTO.Games;
using backend.Enums;

namespace backend.DTO.Users
{
    public record UserGamesResponse
    (
        List<ProfileGameSummary> GameHistory,
        int TotalPages 
    );
    public record ProfileGameSummary
    (
        string GameId,
        string? WinnerNickname,
        string Date,
        string ProfileNickname,
        GameType GameType,

        UserGameSummary WhitePlayer,
        UserGameSummary BlackPlayer

    );
     
}


