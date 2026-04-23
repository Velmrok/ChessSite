using System.ComponentModel.DataAnnotations;

namespace backend.DTO.Users;
public record UpdateUserBioRequest(
    [Required(ErrorMessage ="bioRequired"), MaxLength(200, ErrorMessage ="bioTooLong"), MinLength(0, ErrorMessage ="bioTooShort")]
    string Bio
);