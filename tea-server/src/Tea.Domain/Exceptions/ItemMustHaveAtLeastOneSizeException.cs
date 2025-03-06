namespace Tea.Domain.Exceptions
{
    public class ItemMustHaveAtLeastOneSizeException : BadRequestException
    {
        public ItemMustHaveAtLeastOneSizeException() : base("An item must have at least one size.")
        {
        }
    }
}
