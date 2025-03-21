namespace Tea.Domain.Exceptions
{
    public class EmptyCategoryIdListException : BadRequestException
    {
        public EmptyCategoryIdListException() : base("List categoryId requests cannot be null or empty.")
        {
        }
    }
}
