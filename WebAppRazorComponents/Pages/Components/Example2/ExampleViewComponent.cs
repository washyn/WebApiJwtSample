using Microsoft.AspNetCore.Mvc;

namespace WebAppRazorComponents.Pages.Components.Example2;

public class Example2ViewComponent : ViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        return View("~/Pages/Components/Example2/Default.cshtml");
    }
}