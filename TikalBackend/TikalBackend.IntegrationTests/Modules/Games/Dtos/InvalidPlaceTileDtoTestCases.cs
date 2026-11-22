using RestApi.Controllers.Games.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games.Dtos;

internal class InvalidPlaceTileDtos : TheoryData<PlaceTileDto>
{
    public InvalidPlaceTileDtos()
    {
        // rotation out of range
        Add(new PlaceTileDto() { Q = 1, R = -1, Rotation = -12342 });
        Add(new PlaceTileDto() { Q = 2, R = -3, Rotation = -1 });
        Add(new PlaceTileDto() { Q = 3, R = -9, Rotation = 6 });
        Add(new PlaceTileDto() { Q = 4, R = -10, Rotation = 589343 });
    }
}