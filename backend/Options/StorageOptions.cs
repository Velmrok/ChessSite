// backend/Options/StorageOptions.cs
namespace backend.Options;

public class StorageOptions
{
    public const string SectionName = "Storage";
    
    public string Provider { get; set; } = "Local";
    
    public string AccountId { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string BucketName { get; set; } = "";
    public string PublicBaseUrl { get; set; } = "";
    
    public string LocalBaseUrl { get; set; } = "http://localhost:5062/uploads";
    
    public long MaxFileSizeBytes { get; set; } = 5_242_880;
}