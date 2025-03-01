namespace Tea.Domain.Entities
{
    public class Size
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Price { get; set; }
        public decimal NewPrice { get; set; }
        public int ItemId { get; set; }
        public Item? Item { get; set; }
    }
}
