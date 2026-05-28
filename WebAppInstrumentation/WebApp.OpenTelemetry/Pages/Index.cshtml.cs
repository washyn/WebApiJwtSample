using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.BookStore.Web.Pages;

public class IndexModel : AbpPageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogWarning("This is a warning");
    }
}