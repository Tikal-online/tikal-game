using Users.Contracts.Dtos;

namespace Identity.IntegrationTests.Users.Register;

public static class RegisterDtoTestCases
{
    public static IEnumerable<RegisterDto> InvalidRegisterDtos =>
    [
        // empty username
        new() { Username = "", Password = "Password1!" },
        // empty password
        new() { Username = "Username", Password = "" },
        // password doesnt fulfill criteria
        new() { Username = "MyUser123", Password = "password" },
        // username too long
        new()
        {
            Username = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nisl.",
            Password = "DS39§sdl235!"
        }
    ];

    public static IEnumerable<RegisterDto> ValidRegisterDtos =>
    [
        new() { Username = "Username", Password = "Password1!" },
        new() { Username = "User123", Password = "SuperSecureSecret123?" },
        new() { Username = "g0R^V5QvyhKBnXsqMc~oxLPP^!-CRP", Password = "~kJ~~RjXd~9t>c4n,zCxHAg#mcu%UM" },
        new() { Username = "*Mx7Czck6ANDLA^.9n6g6F8>CDc1CA", Password = "0]~4s?wPEpq].Y41]8Fe=J+moc5xA3jh" }
    ];
}