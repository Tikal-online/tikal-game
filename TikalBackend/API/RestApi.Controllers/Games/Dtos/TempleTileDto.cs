namespace RestApi.Controllers.Games.Dtos;

public sealed record TempleTileDto : TileDto
{
    public required int TempleLevel { get; set; }
}