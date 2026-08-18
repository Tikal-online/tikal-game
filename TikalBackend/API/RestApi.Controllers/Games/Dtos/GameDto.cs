namespace RestApi.Controllers.Games.Dtos;

public record GameDto
{
    public required long Id { get; set; }

    public List<GamePlayerDto> Players { get; set; } = [];

    public List<TileDto> Tiles { get; init; } = [];
}