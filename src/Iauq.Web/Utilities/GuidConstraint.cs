using System;
using System.Web;
using System.Web.Routing;

namespace Iauq.Web.Utilities
{
    public class GuidConstraint : IRouteConstraint
    {
        #region IRouteConstraint Members

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
                          RouteDirection routeDirection)
        {
            Guid g;

            var value = values[parameterName].ToString();

            bool isValid = !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out g) && g != Guid.Empty;

            return isValid;
        }

        #endregion
    }
}