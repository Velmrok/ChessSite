

using System.ComponentModel.DataAnnotations;

namespace backend.DTO.Users
{
    public record AddFriendRequest()
    {
        [Required(ErrorMessage = "requiredNickname")]
        public string Nickname { get; init; } = string.Empty;
    }
}