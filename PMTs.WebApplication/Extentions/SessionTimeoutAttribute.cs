//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PMTs.WebApplication.Extentions
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session == null || !filterContext.HttpContext.Session.TryGetValue("UserSessionModel", out byte[] val))
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
                // base.OnActionExecuting(filterContext);

            }
            base.OnActionExecuting(filterContext);
        }

        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    // var cc = context.HttpContext.Session;
        //    //  var cd = context.HttpContext.Session.TryGetValue("TransactionDataModel", out byte[] valc);
        //    //if (context.HttpContext.Session.Keys == null)

        //    if (context.HttpContext.Session == null || !context.HttpContext.Session.TryGetValue("UserSessionModel", out byte[] val))
        //    {
        //        //context.Result =
        //        //    new RedirectToRouteResult(new RouteValueDictionary(new
        //        //    {
        //        //        controller = "Login",
        //        //        action = "Index"
        //        //    }));

        //        //  controller.HttpContext.Response.Redirect("./Login");

        //        //var controller = (LoginController)context.Controller;
        //        //context.Result = controller.RedirectToActions("Index", "Login");
        //        // context.Result = new RedirectToActionResult(nameof(LoginController.Index), "Login", null);

        //        string redirectTo = "~/Login/Logout";
        //        context.Result = new RedirectResult(redirectTo);
        //    }

        //     base.OnActionExecuting(context);
        //}



        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{

        //  var xx =    SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");

        //    //  HttpContext ctx = HttpContext.Current;
        //    if (_session == null)
        //    {
        //        filterContext.Result = new RedirectResult("~/Home/Login");
        //        return;
        //    }
        //    base.OnActionExecuting(filterContext);
        //}
    }
}
