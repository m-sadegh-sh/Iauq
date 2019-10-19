using System.Data.Entity;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class CommentService : ICommentService
    {
        private readonly IDbSet<Comment> _comments;
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _comments = _unitOfWork.Set<Comment>();
        }

        #region ICommentService Members

        public IQueryable<Comment> GetAllComments()
        {
            return _comments;
        }

        public IQueryable<Comment> GetAllCommentsByOwnerId(int ownerId)
        {
            return _comments.Where(c => c.Owner.Id == ownerId);
        }

        public Comment GetCommentById(int commentId)
        {
            return _comments.Find(commentId);
        }

        public void SaveComment(Comment comment)
        {
            _comments.Add(comment);
        }

        public void DeleteComment(Comment comment)
        {
            _comments.Remove(comment);
        }

        #endregion
    }
}