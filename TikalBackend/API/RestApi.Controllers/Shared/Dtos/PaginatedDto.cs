namespace RestApi.Controllers.Shared.Dtos;

public record PaginatedDto<T>
{
    public required T Data { get; set; }

    public required int TotalCount { get; set; }
}