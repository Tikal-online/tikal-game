using Lobbies.Contracts.Models;
using Riok.Mapperly.Abstractions;
using SignalRApi.Hubs.Lobbies.Dtos;

namespace SignalRApi.Hubs.Lobbies.Mappers;

[Mapper]
internal static partial class ChatMessageModelMapper
{
    public static partial ChatMessageDto ChatMessageModelToChatMessageDto(ChatMessageModel chatMessageModel);
}