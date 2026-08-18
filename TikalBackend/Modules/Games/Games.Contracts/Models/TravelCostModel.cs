namespace Games.Contracts.Models;

public readonly record struct TravelCostModel(
    int North,
    int NorthEast,
    int SouthEast,
    int South,
    int SouthWest,
    int NorthWest
);