using Microsoft.AspNetCore.Mvc;

namespace WebAppRazorComponents.Pages.Components.Example1;

public class Example1ViewComponent : ViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        // TODO: add model
        // Add  contributor system inheritable, same as abp configuration
        return View("~/Pages/Components/Example1/Default.cshtml");
        // TODO: replicar el sistema de permisos de abp, la contribucion
    }
}