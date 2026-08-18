using System.Text.Json.Serialization;

namespace RestApi.Controllers.Games.Dtos;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(EmptyTileDto), "empty")]
[JsonDerivedType(typeof(TreasureTileDto), "treasure")]
[JsonDerivedType(typeof(VolcanoTileDto), "volcano")]
[JsonDerivedType(typeof(TempleTileDto), "temple")]
public abstract record TileDto
{
    public required TravelCostDto Costs { get; set; }

    public required HexCoordinateDto Coordinate { get; set; }
}