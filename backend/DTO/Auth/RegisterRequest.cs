
using System.ComponentModel.DataAnnotations;
namespace backend.DTO.Auth;
public class RegisterRequest
{
    [Required(ErrorMessage = "requiredNickname")]
    [MinLength(3, ErrorMessage = "nicknameTooShort")]
    public required string Nickname { get; set; }
    [Required(ErrorMessage = "requiredLogin")]
    [MinLength(3, ErrorMessage = "loginTooShort")]
    public required string Login { get; set; }

    [Required(ErrorMessage = "requiredEmail")]
    [EmailAddress(ErrorMessage = "invalidEmail")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "requiredPassword")]
    [MinLength(6, ErrorMessage = "passwordTooShort")]
    public required string Password { get; set; }

    
}