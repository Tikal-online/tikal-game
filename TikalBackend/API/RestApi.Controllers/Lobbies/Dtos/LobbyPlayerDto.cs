using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Lobbies.Dtos;

public sealed record LobbyPlayerDto
{
    [Required]
    [MaxLength(100)]
    public required string UserId { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Name { get; set; }

    public required bool IsReady { get; set; }

    public required bool IsOwner { get; set; }
}