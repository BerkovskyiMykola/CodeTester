﻿namespace Common.Models.Pagination;

public record PaginationResult<T>
{
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public long TotalCount { get; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public ICollection<T> Rows { get; }

    public PaginationResult(List<T> items, long count, int pageNumber, int pageSize)
    {
        Rows = items;
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}
