using Lobbies.Contracts.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.Api;

namespace SignalRApi.Hubs.Lobbies.GlobalChat;

[Authorize]
[RequireAccount]
public sealed class GlobalChatHub : Hub<GlobalChatClient>
{
    private readonly ISender sender;

    public GlobalChatHub(ISender sender)
    {
        this.sender = sender;
    }

    public async Task SendMessage(string message)
    {
        var command = new SendGlobalChatMessageCommand(message);

        await sender.Send(command);
    }
}