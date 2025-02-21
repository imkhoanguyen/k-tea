namespace Tea.Domain.Exceptions
{
    /*
     * Từ khóa sealed ngăn chặn class này được kế thừa từ class khác
     */
    public sealed class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(int id) : base($"The category with id: {id} doesn't exist in the database.")
        {
        }
    }
}
