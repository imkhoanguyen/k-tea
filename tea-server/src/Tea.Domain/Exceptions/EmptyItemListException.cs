namespace Tea.Domain.Exceptions
{
    public class EmptyItemListException : BadRequestException
    {
        public EmptyItemListException() : base("List items requests cannot be null or empty.")
        {
        }
    }
}
