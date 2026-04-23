using System.ComponentModel.DataAnnotations;

namespace backend.DTO.Users;
public record UpdateUserBioRequest(
    [Required, MaxLength(200), MinLength(0)]
    string Bio
);