using Microsoft.EntityFrameworkCore.Storage;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class UnitOfWork(TeaContext context) : IUnitOfWork, IDisposable
    {
        private IDbContextTransaction _dbContextTransaction;
        private bool _disposed;
        public ICategoryRepository Category { get; private set; } = new CategoryRepository(context);

        public ISizeRepository Size { get; private set; } = new SizeRepository(context);

        public IItemRepository Item { get; private set; } = new ItemRepository(context);

        public async Task BeginTransactionAsync()
        {
            _dbContextTransaction = await context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if(_dbContextTransaction != null)
            {
               await _dbContextTransaction.CommitAsync();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task RollbackTransactionAsync()
        {
            if (_dbContextTransaction != null)
            {
                await _dbContextTransaction.RollbackAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // equal: _dbContextTransaction?.Dispose();
                    if (_dbContextTransaction != null)
                        _dbContextTransaction.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
