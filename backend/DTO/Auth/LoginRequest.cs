
using System.ComponentModel.DataAnnotations;
namespace backend.DTO.Auth;
using backend.CustomAttributes;


[RequireOneOf("Login", "Email")]
public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
   
    [Required]
    public required string Password { get; set; }
 


    
}