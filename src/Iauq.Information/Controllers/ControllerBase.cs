using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using Iauq.Data.Services;
using Iauq.Web.Mvc;
using StructureMap;

namespace Iauq.Information.Controllers
{
    //[Blocker]
    public abstract class ControllerBase : Controller
    {
        protected ILogService Logger { get; private set; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            Logger = ObjectFactory.GetInstance<ILogService>();
        }

        protected bool IsPartial()
        {
            return Request.IsAjaxRequest() || ControllerContext.IsChildAction ||
                   ControllerContext.ParentActionViewContext != null;
        }

        protected ActionResult ViewOrPartialView()
        {
            if (IsPartial())
                return PartialView();

            return View();
        }

        protected ActionResult ViewOrPartialView(object model)
        {
            string actionName = RouteData.GetRequiredString("Action");

            if (IsPartial())
                return PartialView("_" + actionName, model);

            return View(model);
        }

        protected ActionResult ViewOrPartialView(string viewName, object model)
        {
            if (IsPartial())
                return PartialView("_" + viewName, model);

            return View(viewName, model);
        }

        protected bool IsReferrerValid()
        {
            return Request.Url != null && Request.UrlReferrer != null &&
                   Request.Url.AbsolutePath != Request.UrlReferrer.AbsolutePath;
        }

        protected ActionResult AccessDeniedView()
        {
            ControllerContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;

            return View("AccessDenied");
        }

        protected ActionResult EntityNotFoundView()
        {
            ControllerContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;

            return View("EntityNotFound");
        }

        protected ActionResult NotFoundView()
        {
            ControllerContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;

            return View("NotFound");
        }
    }
}