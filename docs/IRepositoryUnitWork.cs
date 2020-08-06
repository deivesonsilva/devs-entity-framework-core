using System;
using System.Threading;
using System.Threading.Tasks;

namespace //[ns-rep]
{
    public interface IRepositoryUnitWork
    {
        Task CompleteAsync();
        Task CompleteAsync(CancellationToken token);
        Task BeginTransactionAsync();
        Task BeginTransactionAsync(CancellationToken token);
        void Commit();
        void Rollback();
    }
}