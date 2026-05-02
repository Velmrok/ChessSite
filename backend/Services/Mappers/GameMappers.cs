using backend.DTO.Common;
using backend.DTO.Games;
using backend.Enums;
using backend.Models;

namespace backend.Services.Mappers
{
    public static class GameMappers
    {
        public static GameSummary MapToGamesResponse(this Game game, string? winnerNickname)
        {

            return new GameSummary
            (
                Id: game.Id.ToString(),
                WhitePlayer: new UserGameSummary
                (
                    Nickname : game.WhitePlayer.Nickname,
                    ProfilePictureUrl : game.WhitePlayer.ProfilePictureUrl,
                    Rating : game.WhitePlayer.Rating.GetRatingByType(game.Type)
                ),
                BlackPlayer: new UserGameSummary
                (
                    Nickname : game.BlackPlayer.Nickname,
                    ProfilePictureUrl : game.BlackPlayer.ProfilePictureUrl,
                    Rating : game.BlackPlayer.Rating.GetRatingByType(game.Type)
                ),
                WinnerNickname: winnerNickname,
                Type: game.Type,
                Status: game.Status,
                FinishedAt: game.FinishedAt,
                Time: game.Time,
                Increment: game.Increment,
                Moves: game.Moves.Split(' ').ToList()
            );
        }
        public static int GetRatingByTime(this User user, int time)
        {
            return time switch
            {
                <= 3 => user.Rating.Bullet,
                <= 5 => user.Rating.Blitz,
                _ => user.Rating.Rapid
            };
        }
        public static int GetRatingByType(this RatingStats ratingStats, GameType type)
        {
            return type switch
            {
                GameType.Blitz => ratingStats.Blitz,
                GameType.Bullet => ratingStats.Bullet,
                GameType.Rapid => ratingStats.Rapid,
                _ => throw new ArgumentException("Invalid game type")
            };
        }
    


    }
}