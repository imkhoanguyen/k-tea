using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.Unauthorizes
{
    public class InvalidTokenException : UnauthorizeException
    {
        public InvalidTokenException(string message) : base(message)
        {
        }
    }
}
