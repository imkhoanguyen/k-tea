namespace Tea.Domain.Exceptions
{
    public class ItemNotFoundException : NotFoundException
    {
        public ItemNotFoundException(int id) : base($"The item with id: {id} doesn't exist in the database.")
        {
        }
    }
}
