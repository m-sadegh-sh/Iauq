using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Iauq.Core.Domain;

namespace Iauq.Core.Utilities
{
    public interface IWebHelper
    {
        Language CurrentLanguage { get; }
        User GetCurrentUser(HttpContextBase httpContext);
        User GetCurrentUser(HttpContext httpContext);
        bool IsInRoleWithRoles(params string[] roles);
        bool IsInRole(string userName,params string[] roles);
        bool IsInRole(User user,params string[] roles);
        string MapUrl(string path);
        string GetIpAddress();
    }
}
