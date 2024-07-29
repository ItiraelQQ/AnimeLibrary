using AnimeLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using static AnimeLibrary.Models.TestModel;

public class MangaController : Controller
{
    private readonly AniListService _aniListService;
    private readonly UserManager<ApplicationUser> _userManager;


    public MangaController(AniListService aniListService, UserManager<ApplicationUser> userManager)
    {
        _aniListService = aniListService;
        _userManager = userManager;
    }
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> Index()
    {
        var trendingManga = await _aniListService.GetTrendingMangaAsync();
        var popularManga = await _aniListService.GetPopularMangaAsync();
        var popularManhwa = await _aniListService.GetPopularManhwaAsync();
        var genres = await _aniListService.GetGenresAsync();
        var viewModel = new MangaViewModel
        {
            TrendingManga = trendingManga ?? new List<MangaViewModel>(),
            PopularManga = popularManga ?? new List<MangaViewModel>(),
            PopularManhwa = popularManhwa ?? new List<MangaViewModel>(),
            Genres = genres
            
        };

        return View(viewModel);
    }
    public async Task<IActionResult> Details(int id)
    {
        // Асинхронно получаем данные об аниме
        var manga = await _aniListService.GetMangaDetails(id);
        if (manga == null)
        {
            return NotFound();
        }
        var relatedManga = await _aniListService.GetRelatedMangaAsync(id);
        // Создаем ViewModel, используя данные аниме
        var viewModel = new MangaViewModel
        {
            Id = manga.Id,
            TitleRomaji = manga.TitleRomaji,
            TitleEnglish = manga.TitleEnglish,
            TitleNative = manga.TitleNative,
            Description = manga.Description,
            Chapters = manga.Chapters,
            Volumes = manga.Volumes,
            Score = manga.Score,
            CoverImage = manga.CoverImage,
            StartDate = manga.StartDate,
            Genres = manga.Genres,
            RelatedManga = relatedManga
        };

        return View(viewModel);
    }
    public async Task<IActionResult> Genre(string genre, int page = 1, int perPage = 15)
    {
        // Асинхронно получаем список манги по жанру
        var mangaListByGenre = await _aniListService.ListMangaGenre(genre, page, perPage);
        if (mangaListByGenre == null || !mangaListByGenre.Any())
        {
            return NotFound();
        }

        // Преобразуем список Media в список MangaViewModel
        var mangaViewModels = mangaListByGenre.Select(media => new MangaViewModel
        {
            Id = media.Id,
            TitleRomaji = media.Title.Romaji,
            TitleEnglish = media.Title.English,
            TitleNative = media.Title.Native,
            CoverImage = media.CoverImage.Large,
            Description = media.Description,
            Genres = media.Genres,
            Score = media.Score,
            Genre = genre,
            Chapters = media.Chapters,
            AverageScore = media.AverageScore
        }).ToList();

        // Получаем список всех жанров для выпадающего меню или других элементов интерфейса
        var allGenres = await _aniListService.GetGenresAsync();

        // Создаем AnimeGenreViewModel и инициализируем его свойства
        var viewModel = new MangaGenreViewModel
        {
            Genre = genre,
            Mangas = mangaViewModels,
            Genres = allGenres ?? new List<string>()
        };

        return View("Genre", viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToViewed(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var manga = await _aniListService.GetMangaDetails(id);
        if (manga == null)
        {
            return NotFound();
        }

        if (user.ViewedMangas == null)
        {
            user.ViewedMangas = new List<int>();
        }

        if (!user.ViewedMangas.Contains(manga.Id))
        {
            user.ViewedMangas.Add(manga.Id);
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction("ViewedContent", "Account");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromViewed(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (user.ViewedMangas == null || !user.ViewedMangas.Contains(id))
        {
            return NotFound();
        }

        user.ViewedMangas.Remove(id);
        await _userManager.UpdateAsync(user);

        return RedirectToAction("ViewedContent", "Account");
    }
    public async Task<IActionResult> Search(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return PartialView("_SearchResults", new List<MangaViewModel>());
        }

        try
        {
            var searchResults = await _aniListService.SearchMangaByTitleAsync(title);
            var mangaList = searchResults["Page"]["media"].ToObject<List<Media>>();

            var mangaViewModels = mangaList.Select(media => new MangaViewModel
            {
                Id = media.Id,
                TitleRomaji = media.Title?.Romaji,
                TitleEnglish = media.Title?.English,
                TitleNative = media.Title?.Native,
                CoverImage = media.CoverImage?.Large,
                Description = media.Description,
                Genres = media.Genres,
                Score = media.Score.HasValue ? media.Score.Value / 10.0 : (double?)null,
                AverageScore = media.AverageScore,
            }).ToList();

            return PartialView("_SearchResults", mangaViewModels);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return PartialView("_SearchResults", new List<MangaViewModel>());
        }
    }
    public async Task<IActionResult> LoadMoreManga(string genre, int page = 1, int perPage = 15)
    {
        // Асинхронно получаем список аниме по жанру и странице
        var mangaListByGenre = await _aniListService.ListMangaGenre(genre, page, perPage);
        if (mangaListByGenre == null || !mangaListByGenre.Any())
        {
            return NotFound();
        }

        // Преобразуем список Media в список AnimeViewModel
        var mangaViewModels = mangaListByGenre.Select(media => new MangaViewModel
        {
            Id = media.Id,
            TitleRomaji = media.Title.Romaji,
            TitleEnglish = media.Title.English,
            TitleNative = media.Title.Native,
            CoverImage = media.CoverImage.Large,
            Description = media.Description,
            Genres = media.Genres,
            Genre = genre,
            Score = media.Score,
            Chapters = media.Chapters,
            AverageScore = media.AverageScore,
        }).ToList();

        return PartialView("_MangaListPartial", mangaViewModels);
    }

}
