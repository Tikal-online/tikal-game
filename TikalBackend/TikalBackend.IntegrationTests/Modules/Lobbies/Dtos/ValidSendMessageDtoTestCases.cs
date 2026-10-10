using RestApi.Controllers.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

internal class ValidSendMessageDtos : TheoryData<SendMessageDto>
{
    public ValidSendMessageDtos()
    {
        Add(new SendMessageDto() { Message = "Hi" });
        Add(new SendMessageDto() { Message = "Hello this is a test message!!111" });
        Add(new SendMessageDto() { Message = "K5+/Mg/E2q4}!iy/Z?MR2L*_hie0&L" });
    }
}