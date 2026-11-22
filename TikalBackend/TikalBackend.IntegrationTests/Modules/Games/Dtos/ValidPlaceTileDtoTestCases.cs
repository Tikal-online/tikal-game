using RestApi.Controllers.Games.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Games.Dtos;

internal class ValidPlaceTileDtos : TheoryData<PlaceTileDto>
{
    public ValidPlaceTileDtos()
    {
        Add(new PlaceTileDto() { Q = 1, R = -1, Rotation = 0 });
        Add(new PlaceTileDto() { Q = 2, R = -3, Rotation = 3 });
        Add(new PlaceTileDto() { Q = 3, R = -9, Rotation = 5 });
    }
}