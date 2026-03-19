using Users.Contracts.Commands;

namespace Users.Application.Tests.Usecases.Refresh;

internal static class RefreshCommandTestCases
{
    public static IEnumerable<RefreshCommand> ValidRefreshCommands =>
    [
        new(""),
        new("refresh_token"),
        new(
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI3NDNlYTllNy1jMzgzLTQwNjItYjFiYi04MmVhOWNkM2FjM2UiLCJzdWIiOiIxIiwibmFtZSI6InVzZXJuYW1lIiwibmJmIjoxNzczOTQ3MjI4LCJleHAiOjE3NzM5NDc1MjgsImlhdCI6MTc3Mzk0NzIyOCwiaXNzIjoiaHR0cHM6Ly9hdXRoLnRpa2Fsb25saW5lLmNvbSIsImF1ZCI6Imh0dHBzOi8vdGlrYWxvbmxpbmUuY29tIn0.0y1EnpXp2CLHUBnJoZr6didEjHrVSAopRa8716QiAWM"
        )
    ];
}