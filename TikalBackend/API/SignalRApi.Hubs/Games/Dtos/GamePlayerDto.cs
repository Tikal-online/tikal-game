using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SignalRApi.Hubs.Lobbies.Enums;

namespace SignalRApi.Hubs.Games.Dtos;

public sealed record GamePlayerDto
{
    [Required]
    [MaxLength(100)]
    public required string UserId { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ColourDto Colour { get; set; }

    public required int Points { get; set; }

    public required int TurnPosition { get; set; }
}