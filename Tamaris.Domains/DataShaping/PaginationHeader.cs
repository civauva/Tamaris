namespace Tamaris.Domains.DataShaping
{
    public class PaginationHeader
    {
        // 		0	'{"totalCount":5,"pageSize":10,"currentPage":1,"totalPages":1,"hasPreviousPage":false,"hasNextPage":false,"previousPageLink":"","nextPageLink":""}'	string

        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
