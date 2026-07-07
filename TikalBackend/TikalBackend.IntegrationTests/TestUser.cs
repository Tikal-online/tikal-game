namespace TikalBackend.IntegrationTests;

internal sealed record TestUser(string UserId, string Name)
{
    public static readonly TestUser Default = new("7e2c296a-5af3-402e-9c80-6cc0a9548097", "TestUser");

    public static readonly TestUser TestUser1 = new("92037a55-af69-4304-908f-267c3309e368", "TestUser1");

    public static readonly TestUser TestUser2 = new("fff21445-42e9-420c-953e-5aa7208fc053", "TestUser2");

    public static readonly TestUser TestUser3 = new("6363da98-2de5-40c5-88aa-44a79be36b29", "TestUser3");

    public static readonly TestUser TestUser4 = new("49b2a334-e3db-423e-9267-16373d150f45", "TestUser4");
}