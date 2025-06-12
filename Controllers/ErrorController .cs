using Microsoft.AspNetCore.Mvc;

namespace Village_Manager.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/NotFound")]
        public IActionResult NotFoundPage()
        {
            return View("~/Views/Shared/404.cshtml");
        }

        [Route("Error/Internal")]
        public IActionResult InternalError()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
