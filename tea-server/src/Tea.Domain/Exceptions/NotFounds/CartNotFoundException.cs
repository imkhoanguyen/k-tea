namespace Tea.Domain.Exceptions.NotFounds
{
    public class CartNotFoundException : NotFoundException
    {
        public CartNotFoundException(string message) : base(message)
        {
        }
    }
}
