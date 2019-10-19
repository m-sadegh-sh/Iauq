using System.Data.Entity;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDbSet<User> _users;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _users = _unitOfWork.Set<User>();
        }

        #region IUserService Members

        public IQueryable<User> GetAllUsers()
        {
            return _users;
        }

        public User GetUserById(int userId)
        {
            return _users.Find(userId);
        }

        public User GetUserByUserName(string userName)
        {
            return _users.FirstOrDefault(u => u.UserName == userName);
        }

        public bool ValidateUser(string userName, string password, string securityToken)
        {
            return _users.Any(u => u.UserName == userName && u.Password == password && u.SecurityToken == securityToken);
        }

        public void SaveUser(User user)
        {
            _users.Add(user);
        }

        public void DeleteUser(User user)
        {
            _users.Remove(user);
        }

        public void ChangePassword(string userName, string newPassword)
        {
            User user = GetUserByUserName(userName);

            user.Password = newPassword;
            _unitOfWork.SaveChanges();
        }

        #endregion
    }
}