using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using PMTs.WebApplication.Extentions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class LanguageController : Controller
    {

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            returnUrl = "/pmtsappproduction" + returnUrl; // Production
            //returnUrl = "/pmtsapp" + returnUrl;// Test
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

    }
}