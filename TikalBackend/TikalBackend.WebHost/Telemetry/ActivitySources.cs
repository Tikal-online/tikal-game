using System.Diagnostics;

namespace TikalBackend.WebHost.Telemetry;

internal static class ActivitySources
{
    public const string ControllerSourceName = "Tikal.WebHost.Controller";
    public const string MediatRSourceName = "Tikal.WebHost.MediatR";

    public static readonly ActivitySource ControllerSource = new(ControllerSourceName);
    public static readonly ActivitySource MediatRSource = new(MediatRSourceName);
}