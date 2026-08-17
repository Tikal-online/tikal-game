using Games.Domain.Entities;
using Games.Domain.Errors;
using Games.Domain.Types;
using OneOf;

namespace Games.Domain.Extensions;

public static class TileListExtensions
{
    extension(List<Tile> tiles)
    {
        public OneOf<int, NoPathFound> GetTravelCost(HexCoordinate start, HexCoordinate goal)
        {
            var dict = tiles.ToDictionary(tile => tile.Coordinate);

            return dict.GetTravelCost(start, goal);
        }
    }
}