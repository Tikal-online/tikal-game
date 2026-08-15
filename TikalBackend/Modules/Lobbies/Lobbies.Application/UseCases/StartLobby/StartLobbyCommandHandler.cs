using Games.Contracts.Commands;
using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using MediatR;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Application.DataAccess;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.StartLobby;

internal sealed class StartLobbyCommandHandler
    : CommandHandler<StartLobbyCommand, OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby, LobbyCannotBeStarted>>
{
    private readonly LobbyRepository lobbyRepository;

    private readonly UnitOfWork unitOfWork;

    private readonly ISender sender;

    private readonly TransactionHandler transactionHandler;

    private readonly AccountContext accountContext;

    public StartLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        UnitOfWork unitOfWork,
        ISender sender,
        TransactionHandler transactionHandler,
        AccountContext accountContext
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.unitOfWork = unitOfWork;
        this.sender = sender;
        this.transactionHandler = transactionHandler;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby, LobbyCannotBeStarted>> Handle(
        StartLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var lobby = await lobbyRepository.GetByIdAsync(request.LobbyId);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        var player = lobby.GetPlayer(accountContext.Account.UserId);

        if (player is null)
        {
            return new PlayerNotInGivenLobby(request.LobbyId);
        }

        if (!lobby.CanBeStarted)
        {
            return new LobbyCannotBeStarted(request.LobbyId);
        }

        lobby.Start();

        var playerDictionary = PlayerMapper.PlayersToDictionary(lobby.Players);

        return await transactionHandler.CommitScopedTransaction(async ct =>
            {
                await sender.Send(new CreateGameCommand(lobby.Id, playerDictionary), ct);

                await unitOfWork.SaveChangesAsync(ct);

                return new Success();
            },
            cancellationToken);
    }
}