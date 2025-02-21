namespace Tea.Domain.Common
{
    public class PaginationRequest : BaseRequest
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; }
        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
