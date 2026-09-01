using Microsoft.AspNetCore.Mvc;

namespace SharedViews.Vcs;

public class LogoViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
