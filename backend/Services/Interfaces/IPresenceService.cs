namespace backend.Services.Interfaces;
public interface IPresenceService
{
    Task<HashSet<Guid>> GetOnlineIdsAsync(IEnumerable<Guid> userIds);
    Task<bool> IsOnlineAsync(Guid userId);
    Task SetOnlineAsync(Guid userId);
    Task<int> GetOnlineCountAsync();
    Task CleanUpAsync();
    Task FlushLastActiveAsync();
}