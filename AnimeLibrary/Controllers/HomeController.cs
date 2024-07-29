using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AnimeLibrary.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly AniListService _aniListService;
    private readonly ApplicationDbContext _context;

    public HomeController(AniListService aniListService, ApplicationDbContext context)
    {
        _aniListService = aniListService;
        _context = context;
    }
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> Index()
    {
        var currentlyAiringAnime = await _aniListService.GetCurrentlyAiringAnime();
        var currentSeasonAnime = await _aniListService.GetCurrentSeasonAnimeAsync();
        var actionAnime = await _aniListService.GetAnimeByGenre("Action");
        var romanceAnime = await _aniListService.GetAnimeByGenre("Romance");
        var comedyAnime = await _aniListService.GetAnimeByGenre("Comedy");
        var musicAnime = await _aniListService.GetAnimeByGenre("Music");
        var dramaAnime = await _aniListService.GetAnimeByGenre("Drama");
        var sportsAnime = await _aniListService.GetAnimeByGenre("Sports");
        var psychoAnime = await _aniListService.GetAnimeByGenre("Psychological");
        var fantasyAnime = await _aniListService.GetAnimeByGenre("Fantasy");
        var horrorAnime = await _aniListService.GetAnimeByGenre("Horror");
        var ecchiAnime = await _aniListService.GetAnimeByGenre("Ecchi");
        var news = _context.News.OrderByDescending(n => n.CreatedAt).Take(5).ToList();
        var genres = await _aniListService.GetGenresAsync();
        var years = await _aniListService.GetYearsAsync();

        var viewModel = new HomeViewModel
        {
            CurrentlyAiring = MapToAnimeViewModel(currentlyAiringAnime),
            CurrentSeason = MapToAnimeViewModel(currentSeasonAnime),
            Action = MapToAnimeViewModel(actionAnime),
            Romance = MapToAnimeViewModel(romanceAnime),
            Comedy = MapToAnimeViewModel(comedyAnime),
            Music = MapToAnimeViewModel(musicAnime),
            Drama = MapToAnimeViewModel(dramaAnime),
            Sports = MapToAnimeViewModel(sportsAnime),
            Psychological = MapToAnimeViewModel(psychoAnime),
            Fantasy = MapToAnimeViewModel(fantasyAnime),
            Horror = MapToAnimeViewModel(horrorAnime),
            Ecchi = MapToAnimeViewModel(ecchiAnime),
            Genres = genres,
            Years = years,
        };

        return View(viewModel);
    }

    private List<AnimeViewModel> MapToAnimeViewModel(List<Media> animeList)
    {
        var result = new List<AnimeViewModel>();

        foreach (var anime in animeList)
        {
            result.Add(new AnimeViewModel
            {
                Id = anime.Id,
                RomajiTitle = anime.Title.Romaji,
                EnglishTitle = anime.Title.English,
                NativeTitle = anime.Title.Native,
                CoverImage = anime.CoverImage.Large
            });
        }

        return result;
    }
    public async Task<IActionResult> RedirectToCorrectDetailPage(int id)
    {
        var url = await _aniListService.GetCorrectDetailPageUrl(id);
        return Redirect(url);
    }
}
