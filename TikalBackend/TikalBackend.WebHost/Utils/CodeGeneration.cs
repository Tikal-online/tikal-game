namespace TikalBackend.WebHost.Utils;

internal static class CodeGeneration
{
    public static bool IsRunningGeneration()
    {
        return Environment.GetCommandLineArgs().Contains("codegen");
    }
}