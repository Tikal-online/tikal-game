using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;

namespace Lobbies.Application.UseCases.SetPlayerNotReady;

public sealed class SetPlayerNotReadyCommandHandler
{
    private readonly PlayerRepository playerRepository;

    private readonly AccountContext accountContext;

    public SetPlayerNotReadyCommandHandler(
        PlayerRepository playerRepository,
        AccountContext accountContext
    )
    {
        this.playerRepository = playerRepository;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, PlayerNotInALobby>> Handle(
        SetPlayerNotReadyCommand request,
        CancellationToken cancellationToken
    )
    {
        var player = await playerRepository.GetByUserId(accountContext.Account.UserId);

        if (player is null)
        {
            return new PlayerNotInALobby();
        }

        player.ReadyDown();

        return new Success();
    }
}