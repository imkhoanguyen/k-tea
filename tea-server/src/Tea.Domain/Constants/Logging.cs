namespace Tea.Domain.Constants
{
    public abstract class Logging
    {
        public const string SaveChangesFailed = "Failed to save changes to the database.";
        public static string IdMismatch(int routeId, int bodyId)
        {
            return $"Id: {routeId} in the route does not match the Id: {bodyId} in the request body.";
        }

        public static string IdMismatch(string routeId, string bodyId)
        {
            return $"Id: {routeId} in the route does not match the Id: {bodyId} in the request body.";
        }
    }
}
