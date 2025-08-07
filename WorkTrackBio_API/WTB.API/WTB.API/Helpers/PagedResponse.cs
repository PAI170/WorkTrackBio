using System.Net;

namespace WTB.API.Helpers
{
    public class PagedResponse<T> : APIResponse<IEnumerable<T>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;

        public PagedResponse()
        {
            Page = 1;
            PageSize = 10;
            TotalRecords = 0;
            TotalPages = 0;
        }

        public static PagedResponse<T> Create(IEnumerable<T> data, int page, int pageSize, int totalRecords, string message = "Success")
        {
            return new PagedResponse<T>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = data,
                Messages = new List<string> { message },
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }

        public static PagedResponse<T> Create(IEnumerable<T> data, int page, int pageSize, int totalRecords, List<string> messages)
        {
            return new PagedResponse<T>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = data,
                Messages = messages,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }
    }
}
