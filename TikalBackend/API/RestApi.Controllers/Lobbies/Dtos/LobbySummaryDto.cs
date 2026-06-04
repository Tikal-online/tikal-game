using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Lobbies.Dtos;

public sealed record LobbySummaryDto
{
    public required long Id { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Name { get; set; }

    [Range(2, 4)]
    public required int MaxPlayers { get; set; }

    [Range(2, 4)]
    public required int CurrentPlayers { get; set; }
}