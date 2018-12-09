using System.Collections.Generic;
using CMSSolutions.Accounts.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.Accounts
{
    [Feature(Constants.Areas.Accounts)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(AccountController));
        }
    }
}