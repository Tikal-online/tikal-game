using RestApi.Controllers.Accounts.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Utils;

namespace TikalBackend.IntegrationTests;

internal abstract class IntegrationTestFixture : TestContainerFixture
{
    private const string createAccountUrl = "Accounts";

    private CustomWebApplicationFactory factory;

    protected HttpClient Client { get; private set; }

    [SetUp]
    public void Setup()
    {
        factory = new CustomWebApplicationFactory(DatabaseContainer.GetConnectionString());
        Client = factory.CreateDefaultClient();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
        factory.Dispose();
    }

    protected Task CreateUserAccount(TestUser user)
    {
        return Client.PostAsyncWithUser(createAccountUrl, user, new CreateAccountDto { Name = user.Name });
    }
}