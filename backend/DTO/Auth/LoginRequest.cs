
using System.ComponentModel.DataAnnotations;
namespace backend.DTO.Auth;
using backend.CustomAttributes;



public class LoginRequest
{
    [Required(ErrorMessage = "requiredLogin")]
    public string Login { get; set; } = string.Empty;
   
    [Required(ErrorMessage = "requiredPassword")]
    public string Password { get; set; } = string.Empty;
 


    
}