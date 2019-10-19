using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface IRoleService
    {
        IQueryable<Role> GetAllRoles();
        Role GetRoleById(int roleId);
        Role GetRoleByName(string name);
        void SaveRole(Role role);
        void DeleteRole(Role role);
    }
}