using Iauq.Information.Controllers;
using Iauq.Information.Helpers;

namespace Iauq.Information.Areas.Administration.Controllers
{
    [CustomAuthorize(Roles = "Administrators")]
    public abstract class AdministrationControllerBase : ControllerBase
    {
    }
}