using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;

namespace Lobbies.Application.UseCases.SetPlayerReady;

public sealed class SetPlayerReadyCommandHandler
{
    private readonly PlayerRepository playerRepository;

    private readonly AccountContext accountContext;

    public SetPlayerReadyCommandHandler(
        PlayerRepository playerRepository,
        AccountContext accountContext
    )
    {
        this.playerRepository = playerRepository;
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

        return new Success();
    }
}