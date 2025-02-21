namespace Tea.Domain.Exceptions
{
    public sealed class IdMismatchException : BadRequestException
    {
        public IdMismatchException(int routeId, int bodyId) 
            : base($"Id: {routeId} in the route does not match the Id: {bodyId} in the request body.")
        {
        }

   
    }
}
