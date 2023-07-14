using System.Diagnostics;
using System.Net.Mime;
using APIASWAN.Utilidad;
using Microsoft.AspNetCore.Mvc;
using WebApplicationPdfGeneration.Models;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace WebApplicationPdfGeneration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConverter _converter;
    private readonly IViewRenderService _renderService;

    public HomeController(ILogger<HomeController> logger, IConverter converter, IViewRenderService renderService)
    {
        _logger = logger;
        _converter = converter;
        _renderService = renderService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Pdf()
    {
        var htmlContent = await _renderService.RederToStringAsync("/Views/SprintReviewTemplate.cshtml","modelll sample text");
        var pdfContentBytes = _converter.Convert(BuildPdfDocument(htmlContent));
        return File(pdfContentBytes, MediaTypeNames.Application.Pdf, "file.pdf");
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public static HtmlToPdfDocument BuildPdfDocument(string htmlContent)
    {
        var globalSettings = new GlobalSettings()
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 5, Bottom = 10, Left = 5, Right = 5 }
        };
        var objSettings = new ObjectSettings()
        {
            PagesCount = true,
            HtmlContent = htmlContent,
            WebSettings = new WebSettings()
            {
                DefaultEncoding = System.Text.Encoding.UTF8.BodyName,
                // UserStyleSheet = cssPath // styles not works
            },
            // Page = "http://google.com/"
            // HeaderSettings = {FontSize = 9, Right = "Pagina [page] de [toPage]", Line = true, Spacing = 2.812}
        };
        
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objSettings}
        };

        return doc;
    }
}