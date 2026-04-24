
using System.ComponentModel.DataAnnotations;

public record UpdateUserProfilePictureRequest(
    [Required(ErrorMessage ="profilePictureRequired")]
    IFormFile ProfilePictureFile
);