using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.Unauthorizes
{
    public class InvalidRefreshTokenException : UnauthorizeException
    {
        public InvalidRefreshTokenException(string message) : base(message)
        {
        }
    }
}
