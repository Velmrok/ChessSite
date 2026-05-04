
using backend.DTO.Auth;
using backend.DTO.Common;
using backend.DTO.Queue;
using backend.Enums;
using ErrorOr;

namespace backend.Services.Interfaces
{
    public interface IQueueService
    {
        Task<ErrorOr<EmptyResponse>> JoinQueueAsync(string? userId, JoinQueuePayload payload);
        Task<ErrorOr<EmptyResponse>> LeaveQueueAsync(string? userId);
        Task<int> GetQueueLengthAsync();
        QueueData GetUserQueueData(string userId);
        
    }
}