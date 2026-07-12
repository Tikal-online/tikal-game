using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Lobbies.Dtos;

public sealed record SendMessageDto
{
    [Required]
    public required string Message { get; set; }
}