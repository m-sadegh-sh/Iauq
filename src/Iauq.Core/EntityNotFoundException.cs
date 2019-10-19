using System;
using System.Linq.Expressions;
using Iauq.Core.Domain;
using Iauq.Core.Extensions;

namespace Iauq.Core
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message)
            : base(message)
        {
        }
    }

    public sealed class EntityNotFoundException<TEntity> : EntityNotFoundException where TEntity : EntityBase
    {
        public EntityNotFoundException(Expression<Func<TEntity, object>> expression)
            : base(
                string.Format("{0} with supplied {1} couldn't be found!", typeof(TEntity).Name,
                              expression.GetPropertyName()))
        {
        }
    }
}