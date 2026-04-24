using ErrorOr;

namespace backend.Services.Interfaces;

public interface IStorageService
{
    Task<ErrorOr<string>> UploadAvatarAsync(Stream imageStream, string userId, string contentType);
    Task<ErrorOr<Success>> DeleteAsync(string objectKey);
}