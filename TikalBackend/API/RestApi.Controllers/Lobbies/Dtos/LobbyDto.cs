using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Lobbies.Dtos;

public sealed record LobbyDto
{
    public required long Id { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Name { get; set; }

    [Range(2, 4)]
    public required int MaxPlayers { get; set; }

    public List<LobbyPlayerDto> Players { get; set; } = [];
}