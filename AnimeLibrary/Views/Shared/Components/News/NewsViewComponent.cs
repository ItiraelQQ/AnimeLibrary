using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class NewsViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public NewsViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var news = await _context.News
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();
        return View(news);
    }
}
