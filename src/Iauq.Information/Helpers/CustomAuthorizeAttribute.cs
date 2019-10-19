using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Iauq.Core.Domain;
using Iauq.Core.Utilities;
using Iauq.Information.DependencyResolution;

namespace Iauq.Information.Helpers
{
    public sealed class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IWebHelper _webHelper;

        public CustomAuthorizeAttribute()
        {
            _webHelper = IoC.Current.GetInstance<IWebHelper>();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            string[] usersSplit = SplitString(Users);

            if (usersSplit.Length > 0 && !usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            string[] rolesSplit = SplitString(Roles);

            if (rolesSplit.Length == 0)
                return true;

            User dbUser = _webHelper.GetCurrentUser(httpContext);

            return _webHelper.IsInRole(dbUser, rolesSplit);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result =
                new RedirectToRouteResult("Log", new RouteValueDictionary(new {Action = "Login", d = filterContext.RequestContext.HttpContext.Request.RawUrl}));
        }

        private static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            IEnumerable<string> split = from piece in original.Split(',')
                                        let trimmed = piece.Trim()
                                        where !String.IsNullOrEmpty(trimmed)
                                        select trimmed;
            return split.ToArray();
        }
    }
}