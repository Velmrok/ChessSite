
using backend.DTO.Queue;
using ErrorOr;

namespace backend.Services.Interfaces
{
    public interface IQueueService
    {
        Task<ErrorOr<Success>> JoinQueueAsync(string? userId, JoinQueuePayload payload);
        Task<ErrorOr<Success>> LeaveQueueAsync(string? userId);
        Task<int> GetQueueLengthAsync();
    }
}