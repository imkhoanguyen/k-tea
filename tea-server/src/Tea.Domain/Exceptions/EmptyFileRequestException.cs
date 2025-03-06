namespace Tea.Domain.Exceptions
{
    public class EmptyFileRequestException : BadRequestException
    {
        public EmptyFileRequestException() : base("File requests cannot be null or empty.")
        {
        }
    }
}
