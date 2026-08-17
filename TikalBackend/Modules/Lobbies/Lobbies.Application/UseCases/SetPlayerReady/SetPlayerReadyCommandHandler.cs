using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.SetPlayerReady;

internal sealed class SetPlayerReadyCommandHandler
    : CommandHandler<SetPlayerReadyCommand, OneOf<Success, PlayerNotInALobby>>
{
    private readonly PlayerRepository playerRepository;

    private readonly UnitOfWork unitOfWork;

    private readonly AccountContext accountContext;

    public SetPlayerReadyCommandHandler(
        PlayerRepository playerRepository,
        UnitOfWork unitOfWork,
        AccountContext accountContext
    )
    {
        this.playerRepository = playerRepository;
        this.unitOfWork = unitOfWork;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, PlayerNotInALobby>> Handle(
        SetPlayerReadyCommand request,
        CancellationToken cancellationToken
    )
    {
        var player = await playerRepository.GetByUserId(accountContext.Account.UserId);

        if (player is null)
        {
            return new PlayerNotInALobby();
        }

        player.ReadyUp();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}