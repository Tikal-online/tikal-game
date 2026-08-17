using Games.Domain.Entities;
using Games.Domain.Errors;
using Games.Domain.Extensions;
using Games.Domain.Tests.Data;
using Games.Domain.Types;

namespace Games.Domain.Tests.Entities;

internal sealed class TilesTests
{
    [TestCaseSource(typeof(TilesPathFinding), nameof(TilesPathFinding.ValidTestCases))]
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
        Assert.AreEqual(expectedCost, cost.Value);
    }

    [TestCaseSource(typeof(TilesPathFinding), nameof(TilesPathFinding.NoPathTestCases))]
    public void GivenTilesWithNoAvailableRoute_WhenGetTravelCost_ThenReturnsNoPathFoundError(
        List<Tile> tiles,
        HexCoordinate start,
        HexCoordinate goal
    )
    {
        // when
        var cost = tiles.GetTravelCost(start, goal);

        // then
        Assert.That(cost.Value, Is.InstanceOf<NoPathFound>());
    }
}