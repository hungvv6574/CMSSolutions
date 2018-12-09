﻿using System.Web.Mvc;
using System.Web.Routing;

namespace CMSSolutions.Web.Routing
{
    public static class RouteExtensions
    {
        public static string GetAreaName(this RouteBase route)
        {
            var routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
            {
                return routeWithArea.Area;
            }

            var castRoute = route as Route;
            if ((castRoute != null) && (castRoute.DataTokens != null))
            {
                return (castRoute.DataTokens["area"] as string);
            }

            return null;
        }

        public static string GetAreaName(this RouteData routeData)
        {
            object area;
            if (routeData.DataTokens.TryGetValue("area", out area))
            {
                return (area as string);
            }
            return routeData.Route.GetAreaName();
        }
    }
}