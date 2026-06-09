using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.BookStore.Web.Pages;

public class IndexModel : AbpPageModel
{
    public IActionResult OnGet()
    {
        return Redirect("~/swagger");
    }
}