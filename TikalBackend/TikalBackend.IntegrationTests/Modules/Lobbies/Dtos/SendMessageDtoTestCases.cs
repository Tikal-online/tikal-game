using RestApi.Controllers.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

internal static class SendMessageDtoTestCases
{
    public static IEnumerable<SendMessageDto> ValidSendMessageDtoCommands =>
    [
        new()
        {
            Message = "Hi"
        },
        new()
        {
            Message = "Hello this is a test message!!111"
        },
        new()
        {
            Message = "K5+/Mg/E2q4}!iy/Z?MR2L*_hie0&L"
        }
    ];
}