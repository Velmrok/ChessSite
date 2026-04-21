namespace backend.Services.Interfaces;
public interface ICacheInvalidationService
{
    Task InvalidateUsersCache();
}