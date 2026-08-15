using Games.Application.DataAccess;
using Games.Application.Mappers;
using Games.Contracts.Commands;
using Games.Domain.Entities;
using OneOf.Types;
using Shared.Contracts.Messaging;

namespace Games.Application.UseCases.CreateGame;

internal sealed class CreateGameCommandHandler
    : CommandHandler<CreateGameCommand, Success>
{
    private readonly GameRepository gameRepository;

    private readonly UnitOfWork unitOfWork;

    public CreateGameCommandHandler(GameRepository gameRepository, UnitOfWork unitOfWork)
    {
        this.gameRepository = gameRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Success> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var players = PlayerMapper.DictionaryToPlayers(request.Players);

        var game = new Game
        {
            LobbyId = request.LobbyId,
            Players = players
        };

        gameRepository.Create(game);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}