using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.NotFounds
{
    public class SizeNotFoundException : NotFoundException
    {
        public SizeNotFoundException(int id) : base($"Không tìm thấy size với id: {id}.")
        {
        }
    }
}
