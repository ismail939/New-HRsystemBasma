using Microsoft.AspNetCore.Mvc;



using HRsystem.Models;

public class ErrorController : Controller
{
    [Route("Home/Error")]
    public IActionResult Error(int? statusCode = null)
    {
        var exceptionFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        var model = new ErrorViewModel
        {
            StatusCode = statusCode ?? 500,
            Message = exceptionFeature?.Error.Message ?? "حدث خطأ غير متوقع",
            Path = exceptionFeature?.Path
        };

        return View(model);
    }
}
