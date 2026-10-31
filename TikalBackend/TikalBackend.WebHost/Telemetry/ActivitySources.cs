using System.Diagnostics;

namespace TikalBackend.WebHost.Telemetry;

internal static class ActivitySources
{
    public const string ControllerSourceName = "Tikal.WebHost.Controller";

    public static readonly ActivitySource ControllerSource = new(ControllerSourceName);
}