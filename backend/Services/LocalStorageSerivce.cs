
using backend.Services.Interfaces;
using ErrorOr;

public class LocalStorageService(IWebHostEnvironment env) : IStorageService
{
    private readonly IWebHostEnvironment _env = env;

    public async Task<ErrorOr<string>> UploadAvatarAsync(Stream stream, string userId, string contentType)
    {
        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "avatars");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{userId}.webp";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream);

        return $"avatars/{fileName}";
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string objectKey)
    {
        var path = Path.Combine(_env.WebRootPath, "uploads", objectKey);
        if (File.Exists(path)) File.Delete(path);
        return new Success();
    }
    public string GetAvatarUrl(string key)
    {
        return $"/api/uploads/avatars/{key}";
    }

}