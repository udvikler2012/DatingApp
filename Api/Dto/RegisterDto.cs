using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class RegisterDto
{
    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }
}
