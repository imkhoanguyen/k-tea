using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.NotFounds
{
    /*
     * Từ khóa sealed ngăn chặn class này được kế thừa từ class khác
     */
    public sealed class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(int id) : base($"Không tìm thấy danh mục với id: {id}.")
        {
        }
    }
}
