namespace Tea.Domain.Exceptions
{
    public sealed class IdMismatchException : BadRequestException
    {
        public IdMismatchException(string message) 
            : base(message)
        {
        }

   
    }
}
