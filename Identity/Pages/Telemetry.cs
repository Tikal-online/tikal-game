using System.Diagnostics.Metrics;

namespace Identity.Pages;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1724 // Type names should not match namespaces

/// <summary>
///     Telemetry helpers for the UI
/// </summary>
public static class Telemetry
{
    private static readonly string ServiceVersion = typeof(Telemetry).Assembly.GetName().Version!.ToString();

    /// <summary>
    ///     Service name for telemetry.
    /// </summary>
    public static readonly string ServiceName = typeof(Telemetry).Assembly.GetName().Name!;

    /// <summary>
    ///     Metrics configuration
    /// </summary>
    public static class Metrics
    {
        /// <summary>
        ///     Meter for the IdentityServer host project
        /// </summary>
        private static readonly Meter Meter = new(ServiceName, ServiceVersion);

        private static readonly Counter<long> UserLoginCounter = Meter.CreateCounter<long>(Counters.LoggedInUsers);

        private static readonly Counter<long> UserLogoutCounter = Meter.CreateCounter<long>(Counters.LoggedOutUsers);

        /// <summary>
        ///     Helper method to increase <see cref="Counters.LoggedInUsers" /> counter.
        /// </summary>
        /// <param name="clientId">Client Id, if available</param>
        /// <param name="idp">Identity provider</param>
        public static void UserLogin(string? clientId, string idp)
        {
            UserLoginCounter.Add(1,
                new KeyValuePair<string, object?>(Tags.Client, clientId),
                new KeyValuePair<string, object?>(Tags.Idp, idp));
        }

        /// <summary>
        ///     Helper method to increase <see cref="Counters.LoggedInUsers" /> counter on failure.
        /// </summary>
        /// <param name="clientId">Client Id, if available</param>
        /// <param name="idp">Identity provider</param>
        /// <param name="error">Error message</param>
        public static void UserLoginFailure(string? clientId, string idp, string error)
        {
            UserLoginCounter.Add(1,
                new KeyValuePair<string, object?>(Tags.Client, clientId),
                new KeyValuePair<string, object?>(Tags.Idp, idp),
                new KeyValuePair<string, object?>(Tags.Error, error));
        }

        /// <summary>
        ///     Helper method to increase the <see cref="Counters.LoggedOutUsers" /> counter.
        /// </summary>
        /// <param name="idp">Idp/authentication scheme for external authentication, or "local" for built-in.</param>
        public static void UserLogout(string? idp)
        {
            UserLogoutCounter.Add(1, new KeyValuePair<string, object?>(Tags.Idp, idp));
        }

        /// <summary>
        ///     Name of Counters
        /// </summary>
        private static class Counters
        {
            public const string LoggedInUsers = "tokenservice.user_login";
            public const string LoggedOutUsers = "tokenservice.user_logout";
        }

        /// <summary>
        ///     Name of tags
        /// </summary>
        private static class Tags
        {
            public const string Client = "client";
            public const string Error = "error";
            public const string Idp = "idp";
        }
    }
}