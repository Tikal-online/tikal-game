using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Users.Dtos;

public sealed record TokenDto
{
    [Required] public required string AccessToken { get; set; }
}