using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace WebApp.Pages;

public class IndexModel : AbpPageModel
{
    public IActionResult OnGet()
    {
        return Redirect("~/swagger");
    }
}