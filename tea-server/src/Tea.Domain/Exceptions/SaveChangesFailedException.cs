namespace Tea.Domain.Exceptions
{
    public sealed class SaveChangesFailedException : BadRequestException
    {
        public SaveChangesFailedException(string entity) : base($"Failed to save changes {entity} to the database.")
        {
        }
    }
}
