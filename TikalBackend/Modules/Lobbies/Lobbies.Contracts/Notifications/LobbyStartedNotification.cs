using MediatR;
using Shared.Contracts.Enums;

namespace Lobbies.Contracts.Notifications;

public sealed record LobbyStartedNotification(long LobbyId, Dictionary<ColourModel, string> Players) : INotification;