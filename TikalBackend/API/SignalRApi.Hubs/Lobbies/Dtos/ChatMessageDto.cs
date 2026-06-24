using System.ComponentModel.DataAnnotations;

namespace SignalRApi.Hubs.Lobbies.Dtos;

public record ChatMessageDto
{
    [Required]
    [MaxLength(30)]
    public required string Username { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Content { get; set; }
}