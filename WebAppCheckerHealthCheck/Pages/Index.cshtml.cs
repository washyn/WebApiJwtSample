using System.Collections;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppChecker.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        _logger.LogInformation("Visited index page...");
        var vars = Environment.GetEnvironmentVariables();
        _logger.LogInformation("All values: {@IDictionary}", vars);
        foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
        {
            _logger.LogInformation($"{env.Key} = {env.Value}");
        }

        MetricsDefinitions.RequestCounter.Add(1);
        // return Redirect("/health-ui");
        return Page();
    }
}