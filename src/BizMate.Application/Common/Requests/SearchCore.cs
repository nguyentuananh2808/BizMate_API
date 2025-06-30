namespace BizMate.Application.Common.Requests
{
    public class SearchCore
    {
        public string? KeySearch { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
