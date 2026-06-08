using System.Reflection;

namespace SignalRApi.Hubs;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}