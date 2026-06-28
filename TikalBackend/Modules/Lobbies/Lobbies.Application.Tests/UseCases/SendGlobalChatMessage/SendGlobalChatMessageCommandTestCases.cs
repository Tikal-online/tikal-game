using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.SendGlobalChatMessage;

internal static class SendGlobalChatMessageCommandTestCases
{
    public static IEnumerable<SendGlobalChatMessageCommand> ValidSendGlobalMessageCommands =>
    [
        new(" "),
        new("Hello this is a test message123!"),
        new("coaseianoiewihfui3429889cn8w98hwinq989qchw8ahcasncinsdf")
    ];
}