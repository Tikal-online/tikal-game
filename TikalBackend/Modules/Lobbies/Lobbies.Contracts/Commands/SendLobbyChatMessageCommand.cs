namespace Lobbies.Contracts.Commands;

public sealed record SendLobbyChatMessageCommand(long LobbyId, string MessageContent);