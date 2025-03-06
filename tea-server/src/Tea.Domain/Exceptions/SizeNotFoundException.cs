namespace Tea.Domain.Exceptions
{
    public class SizeNotFoundException : NotFoundException
    {
        public SizeNotFoundException(int id) : base($"The size with id: {id} doesn't exist in the database.")
        {
        }
    }
}
