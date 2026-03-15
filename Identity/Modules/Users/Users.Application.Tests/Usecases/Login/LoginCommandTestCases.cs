using Users.Contracts.Commands;

namespace Users.Application.Tests.Usecases.Login;

internal static class LoginCommandTestCases
{
    public static IEnumerable<LoginCommand> InvalidLoginCommands =>
    [
        // empty username
        new("", "Password1!"),
        // empty password
        new("Username", ""),
        // password doesnt fulfill criteria
        new("MyUser123", "password"),
        // username too long
        new("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nisl.", "DS39§sdl235!")
    ];

    public static IEnumerable<LoginCommand> ValidLoginCommands =>
    [
        new("Username", "Password1!"),
        new("User123", "SuperSecureSecret123?"),
        new("g0R^V5QvyhKBnXsqMc~oxLPP^!-CRP", "~kJ~~RjXd~9t>c4n,zCxHAg#mcu%UM"),
        new("*Mx7Czck6ANDLA^.9n6g6F8>CDc1CA", "0]~4s?wPEpq].Y41]8Fe=J+moc5xA3jh")
    ];
}