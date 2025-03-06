namespace Tea.Domain.Exceptions
{
    public class ItemMustHaveAtLeastOneCategoryException : BadRequestException
    {
        public ItemMustHaveAtLeastOneCategoryException() : base("An item must have at least one category.")
        {
        }
    }
}
