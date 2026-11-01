namespace Games.Domain.EventData;

public sealed record TempleTileEventData : TileEventData
{
    public required int TempleLevel { get; set; }
}