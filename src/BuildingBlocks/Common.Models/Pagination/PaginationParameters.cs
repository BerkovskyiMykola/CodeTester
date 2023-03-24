namespace Common.Models.Pagination;

public class PaginationParameters
{
    const int maxPageSize = 50;
    const int minPageNumber = 1;

    private int _pageNumber = 1;
    public int PageNumber
    {
        get
        {
            return _pageNumber;
        }
        set
        {
            _pageNumber = value < minPageNumber ? _pageNumber : value;
        }
    }

    private int _pageSize = 10;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = value > maxPageSize ? maxPageSize : value;
        }
    }
}
