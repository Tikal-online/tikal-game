using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;

namespace Lobbies.Application.UseCases.SetPlayerNotReady;

public sealed class SetPlayerNotReadyCommandHandler
{
    private readonly PlayerRepository playerRepository;


    public SetPlayerNotReadyCommandHandler(PlayerRepository playerRepository)
    {
        this.playerRepository = playerRepository;
    }

    public async Task<OneOf<Success, PlayerNotInALobby>> Handle(
        SetPlayerNotReadyCommand request,
        CancellationToken cancellationToken
    )
    {
        var player = await playerRepository.GetByUserId(request.UserId);

        if (player is null)
        {
            return new PlayerNotInALobby();
        }

        player.ReadyDown();

        return new Success();
    }
}