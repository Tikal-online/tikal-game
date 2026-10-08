using RestApi.Controllers.Accounts.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Accounts.Dtos;

internal class ValidCreateAccountDtos : TheoryData<CreateAccountDto>
{
    public ValidCreateAccountDtos()
    {
        Add(new CreateAccountDto() { Name = "AccountName" });
        Add(new CreateAccountDto() { Name = "MyAccount123$%!" });
        Add(new CreateAccountDto() { Name = ":Q1:-X88gQ^9#uL3S7|SwE6T}&y^*c" });
    }
}