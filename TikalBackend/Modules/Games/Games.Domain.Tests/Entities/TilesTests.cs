using Games.Domain.Entities;
using Games.Domain.Errors;
using Games.Domain.Extensions;
using Games.Domain.Tests.Data;
using Games.Domain.Types;

namespace Games.Domain.Tests.Entities;

public sealed class TilesTests
{
    [Theory]
    [ClassData(typeof(ValidTilePaths))]
    public void GivenTilesWithStartAndGoal_WhenGetTravelCost_ThenReturnsExpectedTravelCost(
        List<Tile> tiles,
        HexCoordinate start,
        HexCoordinate goal,
        int expectedCost
    )
    {
        // when
        var cost = tiles.GetTravelCost(start, goal);

        // then
        Assert.Equal(expectedCost, cost.Value);
    }

    [Theory]
    [ClassData(typeof(NoPathTilePaths))]
    public void GivenTilesWithNoAvailableRoute_WhenGetTravelCost_ThenReturnsNoPathFoundError(
        List<Tile> tiles,
        HexCoordinate start,
        HexCoordinate goal
    )
    {
        // when
        var cost = tiles.GetTravelCost(start, goal);

        // then
        Assert.IsType<NoPathFound>(cost.Value);
    }

    [Theory]
    [ClassData(typeof(ValidTilePlacements))]
    public void GivenTilesAndConnectedTile_WhenCanBePlaced_ThenReturnsTrue(List<Tile> tiles, Tile tile)
    {
        // when
        var canBePlaced = tiles.CanBePlaced(tile);

        // then
        Assert.True(canBePlaced);
    }

    [Theory]
    [ClassData(typeof(InvalidTilePlacements))]
    public void GivenTilesAndUnconnectedOrTakenTile_WhenCanBePlaced_ThenReturnsFalse(List<Tile> tiles, Tile tile)
    {
        // when
        var canBePlaced = tiles.CanBePlaced(tile);

        // then
        Assert.False(canBePlaced);
    }
}