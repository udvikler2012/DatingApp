using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public class RegisterDto
{
    [Required]
    public string Displayname { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
    
    [Required]
    [MinLength(4)]
    public string Password { get; set; } = "";
}
