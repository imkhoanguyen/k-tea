namespace Tea.Domain.Entities
{
    public class ItemCategory
    {
        public int ItemId { get; set; }
        public Item? Item { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ItemCategory(int itemId, int categoryId)
        {
            ItemId = itemId;
            CategoryId = categoryId;
        }
    }
}
