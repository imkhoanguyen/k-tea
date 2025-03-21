namespace Tea.Domain.Exceptions.Bases
{
    public abstract class UnauthorizeException : Exception
    {
        protected UnauthorizeException(string message): base(message) { }
    }
}
