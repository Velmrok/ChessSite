using backend.DTO.Common;
using backend.DTO.Games;
using backend.DTO.Users;
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
                    Nickname: game.WhitePlayer.Nickname,
                    ProfilePictureUrl: game.WhitePlayer.ProfilePictureUrl,
                    Rating: game.WhitePlayer.Rating.GetRatingByType(game.Type)
                ),
                BlackPlayer: new UserGameSummary
                (
                    Nickname: game.BlackPlayer.Nickname,
                    ProfilePictureUrl: game.BlackPlayer.ProfilePictureUrl,
                    Rating: game.BlackPlayer.Rating.GetRatingByType(game.Type)
                ),
                WinnerNickname: winnerNickname,
                Type: game.Type,
                Status: game.Status,
                FinishedAt: game.FinishedAt,
                Time: game.Time,
                Increment: game.Increment,
                Moves: [.. game.Moves.Take(6).Select(m => m.San)]
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
        public static GameResponse MapToGameResponse(this Game game)
        {
            return new GameResponse
            (
                Id: game.Id.ToString(),
                WhitePlayer: new UserGameSummary
                (
                    Nickname: game.WhitePlayer.Nickname,
                    ProfilePictureUrl: game.WhitePlayer.ProfilePictureUrl,
                    Rating: game.WhitePlayer.Rating.GetRatingByType(game.Type)
                ),
                BlackPlayer: new UserGameSummary
                (
                    Nickname: game.BlackPlayer.Nickname,
                    ProfilePictureUrl: game.BlackPlayer.ProfilePictureUrl,
                    Rating: game.BlackPlayer.Rating.GetRatingByType(game.Type)
                ),
                WinnerNickname: game.Winner?.Nickname,
                GameStatus: game.Status,
                Time: game.Time,
                IsWhiteTurn: game.IsWhiteTurn,
                Increment: game.Increment,
                Moves: game.Moves,
                GameType: game.Type,
                CurrentBlackTime: 100,
                CurrentWhiteTime: 100,
                Messages: new List<MessageInfo>(),
                DrawOfferedBy: null,
                EndGameReason: null
            );
        
        }
        public static GameActive MapToGameActive(this Game game)
        {
            return new GameActive
            {
                Id = game.Id.ToString(),

                WhitePlayerId = game.WhitePlayer.Id.ToString(),
                WhitePlayerNickname = game.WhitePlayer.Nickname,
                WhitePlayerProfilePictureUrl = game.WhitePlayer.ProfilePictureUrl,
                WhitePlayerRating = game.WhitePlayer.Rating.GetRatingByType(game.Type),
                CurrentWhiteTime = game.Time * 60 * 1000,

                BlackPlayerId = game.BlackPlayer.Id.ToString(),
                BlackPlayerNickname = game.BlackPlayer.Nickname,
                BlackPlayerProfilePictureUrl = game.BlackPlayer.ProfilePictureUrl,
                BlackPlayerRating = game.BlackPlayer.Rating.GetRatingByType(game.Type),
                CurrentBlackTime = game.Time * 60 * 1000,

                IsWhiteTurn = true,

                Time = game.Time,
                Increment = game.Increment,
                GameType = game.Type,
                DrawOfferedBy = null
            };
        }
        public static GameResponse MapToGameResponse(this GameActive gameActive,List<MoveInfo> moves, List<MessageInfo> messages)
        {
            return new GameResponse
            (
                Id: gameActive.Id,
                WhitePlayer: new UserGameSummary
                (
                    Nickname: gameActive.WhitePlayerNickname,
                    ProfilePictureUrl: gameActive.WhitePlayerProfilePictureUrl,
                    Rating: gameActive.WhitePlayerRating
                ),
                BlackPlayer: new UserGameSummary
                (
                    Nickname: gameActive.BlackPlayerNickname,
                    ProfilePictureUrl: gameActive.BlackPlayerProfilePictureUrl,
                    Rating: gameActive.BlackPlayerRating
                ),
                WinnerNickname: null,
                GameStatus: GameStatus.Active,
                Time: gameActive.Time,
                Increment: gameActive.Increment,
                Moves: moves,
                GameType: gameActive.GameType,
                CurrentBlackTime: gameActive.CurrentBlackTime,
                CurrentWhiteTime: gameActive.CurrentWhiteTime,
                IsWhiteTurn: gameActive.IsWhiteTurn,
                Messages: messages,
                DrawOfferedBy: gameActive.DrawOfferedBy,
                EndGameReason: null
            );
        }
        public static ProfileGameSummary MapToUserGameSummary(this Game game, string profileNickname)
        {
            return new
           (
            GameId: game.Id.ToString(),
            WinnerNickname: game.Winner?.Nickname,
            Date: game.FinishedAt?.ToString("yyyy-MM-dd") ?? "N/A",
            ProfileNickname: profileNickname,
            GameType: game.Type,
            WhitePlayer: new UserGameSummary
            (
                Nickname: game.WhitePlayer.Nickname,
                ProfilePictureUrl: game.WhitePlayer.ProfilePictureUrl,
                Rating: game.WhitePlayer.Rating.GetRatingByType(game.Type)
            ),
            BlackPlayer: new UserGameSummary
            (
                Nickname: game.BlackPlayer.Nickname,
                ProfilePictureUrl: game.BlackPlayer.ProfilePictureUrl,
                Rating: game.BlackPlayer.Rating.GetRatingByType(game.Type)
            )
        );
        

        }


    }
}