namespace Users.Application.Queries.Dto;

public sealed record PageDto<T>(
    int PageNumber,
    int PageSize,
    List<T> Items,
    int TotalItems);