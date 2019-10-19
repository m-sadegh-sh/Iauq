using System.Collections.Generic;
using System.Collections.ObjectModel;
using Iauq.Information.Models.Layout;

namespace Iauq.Information.Models.Layout
{
    public class BreadcrumbModels : List<BreadcrumbModel>
    {
        public BreadcrumbModels Add(string title, string routeName, object routeValues = null)
        {
            Add(new BreadcrumbModel {Title = title, RouteName = routeName, RouteValues = routeValues});

            return this;
        }
    }
}