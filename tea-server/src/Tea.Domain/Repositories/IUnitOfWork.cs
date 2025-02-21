namespace Tea.Domain.Repositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        Task<bool> SaveChangesAsync();
    }
}
