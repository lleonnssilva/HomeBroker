using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBroker.Controllers
{
    [Authorize(Policy = "Investidor")]
    public class MinhaContaController : Controller
    {
        [Route("minha-conta")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("minha-conta/carteira")]
        public IActionResult Carteira()
        {
            return View();
        }

        [Route("minha-conta/evolucao")]
        public IActionResult Evolucao()
        {
            return View();
        }
    }
}
