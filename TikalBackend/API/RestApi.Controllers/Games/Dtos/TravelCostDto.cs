namespace RestApi.Controllers.Games.Dtos;

public readonly record struct TravelCostDto(
    int North,
    int NorthEast,
    int SouthEast,
    int South,
    int SouthWest,
    int NorthWest
);