    // backend/Services/R2StorageService.cs
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using backend.Options;
using backend.Services.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace backend.Services;

public class R2StorageService : IStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly StorageOptions _options;
    private readonly ILogger<R2StorageService> _logger;

    public R2StorageService(
        IOptions<StorageOptions> options,
        ILogger<R2StorageService> logger)
    {
        _options = options.Value;
        _logger = logger;

        var config = new AmazonS3Config
        {
            ServiceURL = $"https://{_options.AccountId}.r2.cloudflarestorage.com",
            
            ForcePathStyle = true,
            
            AuthenticationRegion = "auto"
          
        };

        var credentials = new BasicAWSCredentials(
            _options.AccessKey, 
            _options.SecretKey);
            
        _s3 = new AmazonS3Client(credentials, config);
    }
    public string GetAvatarUrl(string key)
    {
        return $"{_options.PublicBaseUrl}/{key}";
    }

    public async Task<ErrorOr<string>> UploadAvatarAsync(
        Stream imageStream, 
        string userId, 
        string contentType)
    {
        var objectKey = $"{userId}.webp";

        try
        {
            using var processedImage = await ProcessImageAsync(imageStream);
            processedImage.Position = 0;
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                InputStream = processedImage,
                ContentType = "image/webp",

                Headers =
                {
                    CacheControl = "public, max-age=31536000"
                },
                DisablePayloadSigning = true,
                UseChunkEncoding = false
            };

            await _s3.PutObjectAsync(request);
            _logger.LogInformation(
                "Avatar uploaded for user {UserId} to key {Key}", 
                userId, objectKey);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, 
                "S3 error uploading avatar for user {UserId}", userId);
            return Error.Failure("uploadFailed", "Failed to upload avatar to storage.");
        }
        return $"{_options.PublicBaseUrl}/{objectKey}";
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string objectKey)
    {
        try
        {
            await _s3.DeleteObjectAsync(_options.BucketName, objectKey);
            return new Success();
        }
        catch (AmazonS3Exception ex)
        {

            _logger.LogWarning(ex, 
                "Failed to delete object {Key} from R2", objectKey);
            return Error.Failure("deleteFailed", "Failed to delete the old avatar from storage.");
        }
    }

    private static async Task<MemoryStream> ProcessImageAsync(Stream input)
    {
        var output = new MemoryStream();
        
        using var image = await Image.LoadAsync(input);
        
    
        var squareSize = Math.Min(image.Width, image.Height);
        image.Mutate(ctx =>
        {
            ctx.Crop(new Rectangle(
                (image.Width - squareSize) / 2,
                (image.Height - squareSize) / 2,
                squareSize,
                squareSize));
                
            ctx.Resize(256, 256, KnownResamplers.Lanczos3);
        });

        await image.SaveAsync(output, new WebpEncoder { Quality = 85 });
        output.Position = 0;
        
        return output;
    }
}