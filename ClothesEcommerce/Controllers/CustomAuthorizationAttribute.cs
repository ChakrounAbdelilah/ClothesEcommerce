using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ClothesEcommerce.Controllers
{
    public class CustomAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            if (filterContext.HttpContext.Session["admin"] == null)
            {
                // User is not logged in, redirect to the login page
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Account" },
                { "action", "Login" }
            });
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}