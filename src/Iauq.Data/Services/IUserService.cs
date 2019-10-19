using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface IUserService
    {
        IQueryable<User> GetAllUsers();
        User GetUserById(int userId);
        User GetUserByUserName(string userName);
        bool ValidateUser(string userName, string password,string securityToken);
        void SaveUser(User user);
        void DeleteUser(User user);
        void ChangePassword(string userName, string newPassword);
    }
}