using backend.DTO.Auth;
using backend.DTO.Common;
using backend.DTO.Home;
using backend.DTO.Users;
using backend.Enums;
using backend.Models;
using System.Linq;
namespace backend.Services.Mappers;
public static class UserMappers
{
    public static GetMeResponse ToGetMeResponse(this User user, List<string> friendNicknames, QueueData queueData)
    {
        return new GetMeResponse(
            Nickname: user.Nickname,
            ProfilePictureUrl: user.ProfilePictureUrl,
            CreatedAt: user.CreatedAt,
            LastActive: user.LastActive,
            Rating: user.Rating,
            FriendNicknames: friendNicknames,
            QueueData: queueData
        );
    }
    public static UserSearchSummary ToUserResponse(this User user, bool isOnline)
    {
        return new UserSearchSummary(
            Nickname: user.Nickname,
            ProfilePictureUrl: user.ProfilePictureUrl,
            IsOnline: isOnline,
            CreatedAt: user.CreatedAt,
            LastActive: user.LastActive,
            Role: user.Role,
            Rating:user.Rating
        );
    }
    public static UserProfileResponse ToUserProfileResponse(this User user,bool isOnline)
    {
        return new UserProfileResponse(
            Nickname: user.Nickname,
            ProfilePictureUrl: user.ProfilePictureUrl,
            Bio: user.ProfileBio,
            IsOnline: isOnline,
            UserInfo: new UserInfo(
                Rating: user.Rating,
                CreatedAt: user.CreatedAt.ToString("yyyy-MM-dd"),
                GamesPlayed: user.GamesPlayed,
                Wins: user.Wins,
                Losses: user.Losses,
                Draws: user.Draws,
                TotalWins: user.Wins.Blitz + user.Wins.Bullet + user.Wins.Rapid,
                TotalLosses: user.Losses.Blitz + user.Losses.Bullet + user.Losses.Rapid,
                TotalDraws: user.Draws.Blitz + user.Draws.Bullet + user.Draws.Rapid
            )
        );
    }
    
   public static FriendProfileSummary ToFriendProfileSummary(this User user,bool isOnline)
    {
        return new FriendProfileSummary(
            Nickname: user.Nickname,
            ProfilePictureUrl: user.ProfilePictureUrl,
            Rating: user.Rating,
            IsOnline: isOnline
        );
    }
    public static FriendOnlineSummary ToFriendOnlineSummary(this User user)
    {
        return new FriendOnlineSummary(
            Nickname: user.Nickname,
            ProfilePictureUrl: user.ProfilePictureUrl
        );
    }
    public static UserLeaderboardSummary ToUserLeaderboardSummary(this User user, GameType gameType)
    {
        return new UserLeaderboardSummary(
            Nickname: user.Nickname,
            ProfilePictureUrl: user.ProfilePictureUrl,
            Rating: user.Rating.GetRatingByType(gameType)
        );
    }
}