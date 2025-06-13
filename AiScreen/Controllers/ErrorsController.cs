using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxSys.Controllers
{
    [AllowAnonymous]
    public class ErrorsController : Controller
    {

        public IActionResult Unauthorize(string message = "")
        {
            ViewBag.Message = message;
            return RedirectToAction("Index", "AIResume");
        }

        public IActionResult NotFound()
        {
            return View(); // Ensure this view exists as discussed
        }

    }
}
