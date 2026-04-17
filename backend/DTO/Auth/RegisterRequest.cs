
using System.ComponentModel.DataAnnotations;
namespace backend.DTO.Auth;
public class RegisterRequest
{
    [Required(ErrorMessage = "requiredNickname")]
    [MinLength(3, ErrorMessage = "nicknameTooShort")]
    public string Nickname { get; set; }
    [Required(ErrorMessage = "requiredLogin")]
    [MinLength(3, ErrorMessage = "loginTooShort")]
    public  string Login { get; set; }

    [Required(ErrorMessage = "requiredEmail")]
    [EmailAddress(ErrorMessage = "invalidEmail")]
    public  string Email { get; set; }

    [Required(ErrorMessage = "requiredPassword")]
    [MinLength(6, ErrorMessage = "passwordTooShort")]
    public  string Password { get; set; }

    
}