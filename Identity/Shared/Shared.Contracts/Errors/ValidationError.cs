namespace Shared.Contracts.Errors;

public record ValidationError(string PropertyName, IReadOnlyList<string> ErrorMessages);