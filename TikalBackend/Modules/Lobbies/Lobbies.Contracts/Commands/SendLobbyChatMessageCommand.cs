namespace Lobbies.Contracts.Commands;

public sealed record SendLobbyChatMessageCommand(string UserId, long LobbyId, string MessageContent);