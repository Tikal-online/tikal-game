namespace TikalBackend.IntegrationTests;

internal sealed record TestUser(string UserId)
{
    public static readonly TestUser Default = new("7e2c296a-5af3-402e-9c80-6cc0a9548097");
}