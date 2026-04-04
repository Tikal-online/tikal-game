using RestApi.Controllers.Accounts.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Accounts.Dtos;

internal static class CreateAccountDtoTestCases
{
    public static IEnumerable<CreateAccountDto> ValidCreateAccountDtos =>
    [
        new() { Name = "AccountName" },
        new() { Name = "MyAccount123$%!" },
        new() { Name = ":Q1:-X88gQ^9#uL3S7|SwE6T}&y^*c" }
    ];

    public static IEnumerable<CreateAccountDto> InvalidCreateAccountDtos =>
    [
        // empty name
        new() { Name = "" },
        new() { Name = "        " },
        // name longer then 30 characters
        new() { Name = "B~xFPU7]8)U~|9tC>91CV8.+$kj_,DY" }
    ];
}