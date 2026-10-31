using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;

namespace Lobbies.Application.UseCases.SetPlayerReady;

public sealed class SetPlayerReadyCommandHandler
{
    private readonly PlayerRepository playerRepository;


    public SetPlayerReadyCommandHandler(PlayerRepository playerRepository)
    {
        this.playerRepository = playerRepository;
    }

    public async Task<OneOf<Success, PlayerNotInALobby>> Handle(
        SetPlayerReadyCommand request,
        CancellationToken cancellationToken
    )
    {
        var player = await playerRepository.GetByUserId(request.UserId);

        if (player is null)
        {
            return new PlayerNotInALobby();
        }

        player.ReadyUp();

        return new Success();
    }
}