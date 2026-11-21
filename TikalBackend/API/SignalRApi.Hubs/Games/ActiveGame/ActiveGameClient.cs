using SignalRApi.Hubs.Games.Dtos;

namespace SignalRApi.Hubs.Games.ActiveGame;

public interface ActiveGameClient
{
    Task InitializationComplete();

    Task PlayerLeft(GamePlayerDto gamePlayerDto);

    Task TurnCompleted(GamePlayerDto nextPlayer);

    Task GameCompleted();
}