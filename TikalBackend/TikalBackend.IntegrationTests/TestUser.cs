namespace TikalBackend.IntegrationTests;

internal sealed record TestUser(string UserId, string Name)
{
    public static readonly TestUser Default = new("7e2c296a-5af3-402e-9c80-6cc0a9548097", "TestUser");

    public static readonly TestUser TestUser1 = new("92037a55-af69-4304-908f-267c3309e368", "TestUser1");

    public static readonly TestUser TestUser2 = new("fff21445-42e9-420c-953e-5aa7208fc053", "TestUser2");
}