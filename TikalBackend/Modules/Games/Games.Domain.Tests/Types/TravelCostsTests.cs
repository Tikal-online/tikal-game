using Games.Domain.Types;

namespace Games.Domain.Tests.Types;

public sealed class TravelCostsTests
{
    private static readonly TravelCosts Costs = new(
        North: 1,
        NorthEast: 2,
        SouthEast: 3,
        South: 4,
        SouthWest: 5,
        NorthWest: 6
    );

    // ------------------------------------------------------------------------
    // Rotate
    // ------------------------------------------------------------------------

    public static TheoryData<int, TravelCosts> RotatedCosts => new()
    {
        { 1, new TravelCosts(North: 6, NorthEast: 1, SouthEast: 2, South: 3, SouthWest: 4, NorthWest: 5) },
        { 2, new TravelCosts(North: 5, NorthEast: 6, SouthEast: 1, South: 2, SouthWest: 3, NorthWest: 4) },
        { 3, new TravelCosts(North: 4, NorthEast: 5, SouthEast: 6, South: 1, SouthWest: 2, NorthWest: 3) },
        { -1, new TravelCosts(North: 2, NorthEast: 3, SouthEast: 4, South: 5, SouthWest: 6, NorthWest: 1) },
        { -2, new TravelCosts(North: 3, NorthEast: 4, SouthEast: 5, South: 6, SouthWest: 1, NorthWest: 2) },
    };

    [Theory]
    [MemberData(nameof(RotatedCosts))]
    public void GivenCosts_WhenRotate_ThenMovesEveryCostByTheSteps(int steps, TravelCosts expected)
    {
        // when
        var rotated = Costs.Rotate(steps);

        // then
        Assert.Equal(expected, rotated);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-6)]
    [InlineData(12)]
    public void GivenCosts_WhenRotateFullTurns_ThenReturnsSameCosts(int steps)
    {
        // when
        var rotated = Costs.Rotate(steps);

        // then
        Assert.Equal(Costs, rotated);
    }

    [Theory]
    [InlineData(5, -1)]
    [InlineData(4, -2)]
    [InlineData(7, 1)]
    [InlineData(-7, -1)]
    [InlineData(13, 1)]
    public void GivenCosts_WhenRotateEquivalentSteps_ThenReturnsSameCosts(int steps, int equivalentSteps)
    {
        // when
        var rotated = Costs.Rotate(steps);

        // then
        Assert.Equal(Costs.Rotate(equivalentSteps), rotated);
    }

    [Theory]
    [InlineData(1, -1)]
    [InlineData(2, -2)]
    [InlineData(4, 2)]
    public void GivenCosts_WhenRotateAndRotateBack_ThenReturnsOriginalCosts(int steps, int backSteps)
    {
        // when
        var rotated = Costs.Rotate(steps).Rotate(backSteps);

        // then
        Assert.Equal(Costs, rotated);
    }
}