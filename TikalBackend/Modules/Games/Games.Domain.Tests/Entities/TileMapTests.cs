using Games.Domain.Entities;
using Games.Domain.Errors;
using Games.Domain.Tests.Data;
using Games.Domain.Types;

namespace Games.Domain.Tests.Entities;

internal sealed class TileMapTests
{
    [TestCaseSource(typeof(TileMapPathFinding), nameof(TileMapPathFinding.ValidTestCases))]
    public void GivenTileMapWithStartAndGoal_WhenGetTravelCost_ThenReturnsExpectedTravelCost(
        TileMap tileMap,
        HexCoordinate start,
        HexCoordinate goal,
        int expectedCost
    )
    {
        // when
        var cost = tileMap.GetTravelCost(start, goal);

        // then
        Assert.AreEqual(expectedCost, cost.Value);
    }

    [TestCaseSource(typeof(TileMapPathFinding), nameof(TileMapPathFinding.NoPathTestCases))]
    public void GivenTileMapWithNoAvailableRoute_WhenGetTravelCost_ThenReturnsNoPathFoundError(
        TileMap tileMap,
        HexCoordinate start,
        HexCoordinate goal
    )
    {
        // when
        var cost = tileMap.GetTravelCost(start, goal);

        // then
        Assert.That(cost.Value, Is.InstanceOf<NoPathFound>());
    }
}