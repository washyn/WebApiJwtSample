using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppEnum.Data;
using WebAppEnum.Ebooks;

namespace WebAppEnum.Pages.Ebooks;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Ebook Ebook { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.Ebooks == null || Ebook == null)
        {
            return Page();
        }

        _context.Ebooks.Add(Ebook);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
