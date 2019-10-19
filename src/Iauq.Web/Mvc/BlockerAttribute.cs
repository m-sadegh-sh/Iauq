using System.Web.Mvc;

namespace Iauq.Web.Mvc
{
    public class BlockerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            string browser = filterContext.HttpContext.Request.Browser.Browser;
            int major = filterContext.HttpContext.Request.Browser.MajorVersion;

            bool isIe6 = browser.ToLowerInvariant() == "ie" && major <= 6;

            if (isIe6)
            {
                if (!(filterContext.RouteData.GetRequiredString("Controller") == "Home" &&
                      filterContext.RouteData.GetRequiredString("Action") == "ServiceBlocked"))
                    filterContext.Result = new ViewResult {ViewName = "ServiceBlocked"};
            }
        }
    }
}