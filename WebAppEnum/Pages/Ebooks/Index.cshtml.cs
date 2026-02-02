using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebAppEnum.Data;
using WebAppEnum.Ebooks;

namespace WebAppEnum.Pages.Ebooks;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Ebook> Ebook { get;set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Ebooks != null)
        {
            Ebook = await _context.Ebooks.ToListAsync();
        }
    }
}
