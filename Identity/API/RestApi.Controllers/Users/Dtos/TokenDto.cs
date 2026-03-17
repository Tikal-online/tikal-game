using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Users.Dtos;

internal sealed record TokenDto
{
    [Required] public required string AccessToken { get; set; }
}