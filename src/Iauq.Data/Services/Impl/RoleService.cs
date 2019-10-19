using System.Data.Entity;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class RoleService : IRoleService
    {
        private readonly IDbSet<Role> _roles;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _roles = _unitOfWork.Set<Role>();
        }

        #region IRoleService Members

        public IQueryable<Role> GetAllRoles()
        {
            return _roles;
        }

        public Role GetRoleById(int roleId)
        {
            return _roles.Find(roleId);
        }

        public Role GetRoleByName(string name)
        {
            return _roles.FirstOrDefault(r => r.Name == name);
        }

        public void SaveRole(Role role)
        {
            _roles.Add(role);
        }

        public void DeleteRole(Role role)
        {
            _roles.Remove(role);
        }

        #endregion
    }
}