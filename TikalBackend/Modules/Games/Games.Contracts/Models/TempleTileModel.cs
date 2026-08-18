namespace Games.Contracts.Models;

public sealed record TempleTileModel : TileModel
{
    public required int TempleLevel { get; set; }
}