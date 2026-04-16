
using System.ComponentModel.DataAnnotations;
namespace backend.DTO.Auth;
using backend.CustomAttributes;


[RequireOneOf("Login", "Email")]
public class LoginRequest
{
    [Required(ErrorMessage = "requiredLogin")]
    public string Login { get; set; } = string.Empty;
   
    [Required(ErrorMessage = "requiredPassword")]
    public required string Password { get; set; }
 


    
}