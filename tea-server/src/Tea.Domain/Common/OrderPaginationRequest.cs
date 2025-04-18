    namespace Tea.Domain.Common
    {
        public class OrderPaginationRequest : PaginationRequest
        {
            public DateTimeOffset? FromDate { get; set; }
            public DateTimeOffset? ToDate { get; set; }
            public double? MinAmount { get; set; }
            public double? MaxAmount { get; set; }
            public string? UserName { get; set; }
        }
    }
