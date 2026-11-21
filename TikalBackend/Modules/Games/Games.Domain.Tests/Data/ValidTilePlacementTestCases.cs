using Games.Domain.Entities;
using Games.Domain.Types;

namespace Games.Domain.Tests.Data;

// the test cases follow the structure: existing tiles, tile to place
internal class ValidTilePlacements : TheoryData<List<Tile>, Tile>
{
    public ValidTilePlacements()
    {
        // only the existing neighbour has a cost on the shared edge
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(North: 1),
                    Coordinate = new HexCoordinate(0, 0)
                }
            ],
            new EmptyTile
            {
                Costs = new TravelCosts(),
                Coordinate = new HexCoordinate(0, -1)
            }
        );
        // only the placed tile has a cost on the shared edge
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(),
                    Coordinate = new HexCoordinate(0, 0)
                }
            ],
            new EmptyTile
            {
                Costs = new TravelCosts(NorthWest: 1),
                Coordinate = new HexCoordinate(1, 0)
            }
        );
        // both tiles have a cost on the shared edge
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(South: 2),
                    Coordinate = new HexCoordinate(0, 0)
                }
            ],
            new TempleTile
            {
                TempleLevel = 1,
                Costs = new TravelCosts(North: 1),
                Coordinate = new HexCoordinate(0, 1)
            }
        );
        // several neighbours, only one of them is connected
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(South: 3),
                    Coordinate = new HexCoordinate(1, -1)
                },
                new VolcanoTile
                {
                    Costs = new TravelCosts(),
                    Coordinate = new HexCoordinate(2, 0)
                }
            ],
            new TreasureTile
            {
                Costs = new TravelCosts(),
                Coordinate = new HexCoordinate(1, 0)
            }
        );
    }
}