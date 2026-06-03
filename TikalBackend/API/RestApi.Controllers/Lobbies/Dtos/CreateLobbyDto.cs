using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Lobbies.Dtos;

public sealed record CreateLobbyDto
{
    [Required]
    [MaxLength(30)]
    public required string Name { get; set; }

    [Range(2, 4)]
    public required int MaxPlayers { get; set; }
}