namespace Identity.Pages;

internal static class Log
{
    private static readonly Action<ILogger, IEnumerable<string>, Exception?> _externalClaims =
        LoggerMessage.Define<IEnumerable<string>>(
            LogLevel.Debug,
            EventIds.ExternalClaims,
            "External claims: {Claims}");

    public static void ExternalClaims(this ILogger logger, IEnumerable<string> claims)
    {
        _externalClaims(logger, claims, null);
    }
}

internal static class EventIds
{
    private const int UIEventsStart = 10000;

    // External Login
    private const int ExternalLoginEventsStart = UIEventsStart + 2000;
    public const int ExternalClaims = ExternalLoginEventsStart + 0;
}