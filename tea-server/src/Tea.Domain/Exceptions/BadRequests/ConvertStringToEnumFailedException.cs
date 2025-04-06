namespace Tea.Domain.Exceptions.BadRequests
{
    public class ConvertStringToEnumFailedException : BadRequestException
    {
        public ConvertStringToEnumFailedException(string message) : base(message)
        {
        }
    }
}
