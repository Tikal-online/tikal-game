using Lobbies.Domain.Entities;
using Shared.Domain.Enums;

namespace Lobbies.Domain.Tests.Data;

public static class PlayerTestCases
{
    public static IEnumerable<Player> ValidPlayerTestCases =>
    [
        new()
        {
            UserId = "bcdc805c-bbc3-49fd-b27d-a51b6a415faa",
            SelectedColour = Colour.Black,
            IsReady = false,
            IsOwner = false
        },
        new()
        {
            UserId = "d1231dd8-fbb2-4913-a47e-21a91901bbee",
            SelectedColour = Colour.Red,
            IsReady = true,
            IsOwner = false
        },
        new()
        {
            UserId = "6d7f3cd6-3065-417d-a3f6-9e5a9ac01511",
            SelectedColour = Colour.Green,
            IsReady = false,
            IsOwner = true
        },
        new()
        {
            UserId = "ab7f9a0e-15ed-4c90-a067-3aa321be0722",
            SelectedColour = Colour.Yellow,
            IsReady = true,
            IsOwner = true
        }
    ];
}