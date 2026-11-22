using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Games.Dtos;

public sealed record PlaceTileDto
{
    public required int Q { get; set; }

    public required int R { get; set; }

    [Range(0, 5)]
    public required int Rotation { get; set; }
}