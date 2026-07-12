using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.SendLobbyChatMessage;

internal static class SendLobbyChatMessageCommandTestCases
{
    public static IEnumerable<SendLobbyChatMessageCommand> ValidSendLobbyMessageCommands =>
    [
        new(" "),
        new("Hello this is a test message123!"),
        new("coaseianoiewihfui3429889cn8w98hwinq989qchw8ahcasncinsdf")
    ];
}