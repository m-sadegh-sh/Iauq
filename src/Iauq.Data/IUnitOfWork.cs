using System.Data.Entity;
using Iauq.Core.Domain;

namespace Iauq.Data
{
    public interface IUnitOfWork
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : EntityBase;
        int GetNextTableIdentity(string tableName);
        int SaveChanges();
    }
}