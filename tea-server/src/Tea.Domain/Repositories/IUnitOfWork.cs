namespace Tea.Domain.Repositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ISizeRepository Size { get; }
        IItemRepository Item { get; }
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void Dispose();
    }
}
