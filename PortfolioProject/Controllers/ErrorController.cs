using Microsoft.AspNetCore.Mvc;

namespace PortfolioProject.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            return statusCode switch
            {
                403 => View("Forbidden"),
                404 => View("NotFound"),
                _ => View("Error")
            };
        }
    }
}
