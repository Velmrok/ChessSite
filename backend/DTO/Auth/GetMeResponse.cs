using backend.DTO.Common;

namespace backend.DTO.Auth;



public record GetMeResponse(
    string Nickname,
    string ProfilePictureUrl,
    DateTime CreatedAt,
    DateTime LastActive,
    RatingStats Rating ,
    List<string> FriendNicknames ,
    QueueData QueueData
);
public record QueueData(
    bool IsInQueue,
    int? Time = null,
    int? Increment = null,
    DateTime? JoinedQueueAt = null
);