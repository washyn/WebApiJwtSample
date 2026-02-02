using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebAppEnum.Data;
using WebAppEnum.Ebooks;

namespace WebAppEnum.Pages.Ebooks;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Ebook Ebook { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null || _context.Ebooks == null)
        {
            return NotFound();
        }

        var ebook = await _context.Ebooks.FirstOrDefaultAsync(m => m.Id == id);
        if (ebook == null)
        {
            return NotFound();
        }
        else
        {
            Ebook = ebook;
        }
        return Page();
    }
}
