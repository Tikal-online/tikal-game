using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

internal class ValidCreateLobbyCommands : TheoryData<CreateLobbyCommand>
{
    public ValidCreateLobbyCommands()
    {
        Add(new CreateLobbyCommand("userId", "username", "LobbyName", 2));
        Add(new CreateLobbyCommand("userId", "username", "MyLobby123", 3));
        Add(new CreateLobbyCommand("userId", "username", "rX`J%trwH3=+v8HQA)]fk:dBNb,!3e", 4));
    }
}