namespace backend.DTO.Common;

public record UserSummary{
    public required string Nickname {get; init;}
    public  required string ProfilePictureUrl {get; init;}
    public int? Rating {get; init;}
    public bool? IsOnline {get; init;}
}