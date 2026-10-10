using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

internal class InvalidCreateLobbyCommands : TheoryData<CreateLobbyCommand>
{
    public InvalidCreateLobbyCommands()
    {
        // empty name
        Add(new CreateLobbyCommand("", 2));
        // name longer than 30 characters
        Add(new CreateLobbyCommand("nGBI7UZ{+grfV!a~@Q0<+?8u#[lPMOg", 2));
        // maxPlayers smaller than 2
        Add(new CreateLobbyCommand("Lobby123", 1));
        Add(new CreateLobbyCommand("MyLobby", -1231));
        // maxPlayers greater then 4
        Add(new CreateLobbyCommand("TestLobby", 5));
        Add(new CreateLobbyCommand("Hello", 3249872));
    }
}