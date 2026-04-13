
using System.ComponentModel.DataAnnotations;
namespace backend.DTO.Auth;
public class RegisterRequest
{
    [Required]
    [MinLength(3)]
    public required string Nickname { get; set; }
    [Required]
    [MinLength(3)]
    public required string Login { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }

    
}