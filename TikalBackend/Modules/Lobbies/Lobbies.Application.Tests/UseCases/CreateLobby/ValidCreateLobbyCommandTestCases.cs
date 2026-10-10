using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

internal class ValidCreateLobbyCommands : TheoryData<CreateLobbyCommand>
{
    public ValidCreateLobbyCommands()
    {
        Add(new CreateLobbyCommand("LobbyName", 2));
        Add(new CreateLobbyCommand("MyLobby123", 3));
        Add(new CreateLobbyCommand("rX`J%trwH3=+v8HQA)]fk:dBNb,!3e", 4));
    }
}