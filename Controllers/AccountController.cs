using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace HomeBroker.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;



        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            var clientId = _configuration["Cognito:ClientId"];
            var redirectUri = _configuration["Cognito:RedirectUri"];
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "OpenID Connect");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var redirectUrl = Url.Action("Index", "Home", null, Request.Scheme);
            var cognitoLogoutUrl = $"{_configuration["Cognito:UrlAuth"]}/logout?client_id={_configuration["Cognito:ClientId"]}&logout_uri={redirectUrl}";

            return Redirect(cognitoLogoutUrl);
        }

    }
}
