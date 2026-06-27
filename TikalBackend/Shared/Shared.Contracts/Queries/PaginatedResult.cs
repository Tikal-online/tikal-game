namespace Shared.Contracts.Queries;

public record PaginatedResult<T>
{
    public required T Data { get; set; }

    public required int TotalCount { get; set; }
}