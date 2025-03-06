namespace Tea.Domain.Exceptions
{
    public class EmptySizeListException : BadRequestException
    {
        public EmptySizeListException() : base("List size requests cannot be null or empty.")
        {
        }
    }
}
