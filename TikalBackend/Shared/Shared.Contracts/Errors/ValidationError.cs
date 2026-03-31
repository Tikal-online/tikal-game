namespace Shared.Contracts.Errors;

public sealed record ValidationError(string PropertyName, IReadOnlyList<string> ErrorMessages);