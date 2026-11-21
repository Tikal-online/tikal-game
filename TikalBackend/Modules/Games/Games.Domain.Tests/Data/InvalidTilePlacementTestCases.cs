using Games.Domain.Entities;
using Games.Domain.Types;

namespace Games.Domain.Tests.Data;

// the test cases follow the structure: existing tiles, tile to place
internal class InvalidTilePlacements : TheoryData<List<Tile>, Tile>
{
    public InvalidTilePlacements()
    {
        // no tiles on the board yet
        Add(
            [],
            new EmptyTile
            {
                Costs = new TravelCosts(North: 1),
                Coordinate = new HexCoordinate(0, 0)
            }
        );
        // no neighbouring tile
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(North: 1, SouthEast: 1),
                    Coordinate = new HexCoordinate(0, 0)
                }
            ],
            new EmptyTile
            {
                Costs = new TravelCosts(NorthWest: 1),
                Coordinate = new HexCoordinate(3, 0)
            }
        );
        // a neighbour, but neither tile has a cost on the shared edge
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(South: 1),
                    Coordinate = new HexCoordinate(0, 0)
                }
            ],
            new EmptyTile
            {
                Costs = new TravelCosts(SouthWest: 1),
                Coordinate = new HexCoordinate(0, -1)
            }
        );
        // several neighbours, none of them is connected
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(NorthEast: 2),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(North: 1),
                    Coordinate = new HexCoordinate(1, -1)
                }
            ],
            new EmptyTile
            {
                Costs = new TravelCosts(SouthEast: 1),
                Coordinate = new HexCoordinate(1, 0)
            }
        );
        // the place is already taken, even though the neighbours are connected
        Add(
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(North: 1),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(South: 1),
                    Coordinate = new HexCoordinate(0, -1)
                }
            ],
            new EmptyTile
            {
                Costs = new TravelCosts(South: 1),
                Coordinate = new HexCoordinate(0, -1)
            }
        );
    }
}