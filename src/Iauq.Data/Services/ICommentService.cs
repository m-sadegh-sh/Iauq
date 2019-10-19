using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface ICommentService
    {
        IQueryable<Comment> GetAllComments();
        IQueryable<Comment> GetAllCommentsByOwnerId(int ownerId);
        Comment GetCommentById(int commentId);
        void SaveComment(Comment comment);
        void DeleteComment(Comment comment);
    }
}