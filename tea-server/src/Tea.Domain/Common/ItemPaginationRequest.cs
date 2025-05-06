namespace Tea.Domain.Common
{
    public class ItemPaginationRequest : PaginationRequest
    {
        public int CategoryId { get; set; }
        public bool? IsFeatured { get; set; }
    }
}
