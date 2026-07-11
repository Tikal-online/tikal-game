using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.JoinLobby;

internal sealed class JoinLobbyCommandHandler
    : CommandHandler<JoinLobbyCommand, OneOf<Success, PlayerAlreadyInALobby, LobbyNotFound, LobbyFull>>
{
    private readonly LobbyRepository lobbyRepository;

    private readonly PlayerQueryContext playerQueryContext;

    private readonly UnitOfWork unitOfWork;

    private readonly AccountContext accountContext;

    public JoinLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        PlayerQueryContext playerQueryContext,
        UnitOfWork unitOfWork,
        AccountContext accountContext
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.playerQueryContext = playerQueryContext;
        this.unitOfWork = unitOfWork;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, PlayerAlreadyInALobby, LobbyNotFound, LobbyFull>> Handle(
        JoinLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var playerAlreadyInALobby = await playerQueryContext.PlayerExists(accountContext.Account.UserId);

        if (playerAlreadyInALobby)
        {
            return new PlayerAlreadyInALobby();
        }

        var lobby = await lobbyRepository.GetByIdAsync(request.LobbyId);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        if (lobby.IsFull)
        {
            return new LobbyFull(lobby.Id);
        }

        var colour = lobby.GetUnusedColour();

        var player = new Player
        {
            UserId = accountContext.Account.UserId,
            SelectedColour = colour,
            IsOwner = false,
            IsReady = false
        };

        lobby.AddPlayer(player);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}