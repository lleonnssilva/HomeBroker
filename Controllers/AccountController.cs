using Microsoft.AspNetCore.Mvc;

namespace HomeBroker.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Login()
        {
            if (HttpContext.User.Identity == null || !HttpContext.User.Identity.IsAuthenticated)
                return Challenge("oidc");
            return Redirect("~/Home/Index");
        }

        public ActionResult Logout()
        {

            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                return new SignOutResult(new[] { "oidc", "cookie" });
            return RedirectToAction(Url.Action("Index", "Home", null, Request.Scheme));

        }
        public ActionResult AccessDenied()
        {

            return View();
        }

    }
}
