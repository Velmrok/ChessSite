
using System.ComponentModel.DataAnnotations;
namespace backend.DTO.Auth;
using backend.CustomAttributes;


[RequireOneOf("Login", "Email")]
public class LoginRequest
{
    [Required]
    public string Login { get; set; } = string.Empty;
   
    [Required]
    public required string Password { get; set; }
 


    
}