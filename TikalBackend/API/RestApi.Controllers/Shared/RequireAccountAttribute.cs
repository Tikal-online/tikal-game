namespace RestApi.Controllers.Shared;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class RequireAccountAttribute : Attribute
{
}