using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AnimeLibrary.Models;

public class NewsController : Controller
{
    private readonly ApplicationDbContext _context;

    public NewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: News
    public IActionResult Index()
    {
        var news = _context.News.OrderByDescending(n => n.CreatedAt).ToList();
        return View(news);
    }

    // GET: News/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: News/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content,FullText,ImageUrl")] News news)
    {
        if (ModelState.IsValid)
        {
            news.CreatedAt = DateTime.Now;
            _context.Add(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(news);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var newsItem = await _context.News.FindAsync(id);
        if (newsItem != null)
        {
            _context.News.Remove(newsItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Details(int id)
    {
        var newsItem = await _context.News.FindAsync(id);
        if (newsItem == null)
        {
            return NotFound();
        }

        return View(newsItem);
    }
}
