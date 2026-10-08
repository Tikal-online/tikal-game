using RestApi.Controllers.Accounts.Dtos;
using Xunit;

namespace TikalBackend.IntegrationTests.Modules.Accounts.Dtos;

internal class InvalidCreateAccountDtos : TheoryData<CreateAccountDto>
{
    public InvalidCreateAccountDtos()
    {
        // empty name
        Add(new CreateAccountDto() { Name = "" });
        Add(new CreateAccountDto() { Name = "     " });
        // name longer then 30 characters
        Add(new CreateAccountDto() { Name = "B~xFPU7]8)U~|9tC>91CV8.+$kj_,DY" });
    }
}