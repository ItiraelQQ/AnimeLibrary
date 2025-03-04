using AnimeLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AnimeLibrary.Controllers
{
    public class AnimeController : Controller
    {
        private readonly AniListService _aniListService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnimeController(AniListService aniListService, UserManager<ApplicationUser> userManager)
        {
            _aniListService = aniListService;
            _userManager = userManager;
        }
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Details(int id)
        {
            var anime = await _aniListService.GetAnimeDetails(id);
            if (anime == null)
            {
                return NotFound();
            }
            var relatedAnime = await _aniListService.GetRelatedAnimeAsync(id);
            var viewModel = new AnimeViewModel
            {
                Id = anime.Id,
                RomajiTitle = anime.RomajiTitle,
                EnglishTitle = anime.EnglishTitle,
                NativeTitle = anime.NativeTitle,
                CoverImage = anime.CoverImage,
                Description = anime.Description,
                Genres = anime.Genres,
                Score = anime.Score,
                StartDate = anime.StartDate,
                Episodes = anime.Episodes,
                RelatedAnime = relatedAnime,
                Characters = anime.Characters,
                Trailer = anime.Trailer,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Genre(string genre, int page = 1, int perPage = 15)
        {
            // Асинхронно получаем список аниме по жанру и странице
            var animeListByGenre = await _aniListService.ListAnimeGenre(genre, page, perPage);
            if (animeListByGenre == null || !animeListByGenre.Any())
            {
                return NotFound();
            }

            // Преобразуем список Media в список AnimeViewModel
            var animeViewModels = animeListByGenre.Select(media => new AnimeViewModel
            {
                Id = media.Id,
                RomajiTitle = media.Title.Romaji,
                EnglishTitle = media.Title.English,
                NativeTitle = media.Title.Native,
                CoverImage = media.CoverImage.Large,
                Description = media.Description,
                Genres = media.Genres,
                Genre = genre,
                Score = media.Score,
                Episodes = media.Episodes,
                AverageScore = media.AverageScore,
            }).ToList();

            var allGenres = await _aniListService.GetGenresAsync();

            var viewModel = new AnimeGenreViewModel
            {
                Genre = genre,
                Animes = animeViewModels,
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

            var anime = await _aniListService.GetAnimeDetails(id);
            if (anime == null)
            {
                return NotFound();
            }

            if (user.ViewedAnimes == null)
            {
                user.ViewedAnimes = new List<int>();
            }

            if (!user.ViewedAnimes.Contains(anime.Id))
            {
                user.ViewedAnimes.Add(anime.Id);
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

            if (user.ViewedAnimes == null || !user.ViewedAnimes.Contains(id))
            {
                return NotFound();
            }

            user.ViewedAnimes.Remove(id);
            await _userManager.UpdateAsync(user);

            return RedirectToAction("ViewedContent", "Account");
        }

        public async Task<IActionResult> Search(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return PartialView("_SearchResults", new List<AnimeViewModel>());
            }

            try
            {
                var searchResults = await _aniListService.SearchAnimeByTitleAsync(title);
                var animeList = searchResults["Page"]["media"].ToObject<List<Media>>();

                var animeViewModels = animeList.Select(media => new AnimeViewModel
                {
                    Id = media.Id,
                    RomajiTitle = media.Title?.Romaji,
                    EnglishTitle = media.Title?.English,
                    NativeTitle = media.Title?.Native,
                    CoverImage = media.CoverImage?.Large,
                    Description = media.Description,
                    Genres = media.Genres,
                    Score = media.Score.HasValue ? media.Score.Value / 10.0 : (double?)null,
                    Episodes = media.Episodes,
                    AverageScore = media.AverageScore,
                }).ToList();

                return PartialView("_SearchResults", animeViewModels);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("_SearchResults", new List<AnimeViewModel>());
            }
        }

        public async Task<IActionResult> LoadMoreAnime(string genre, int page = 1, int perPage = 15)
        {
            var animeListByGenre = await _aniListService.ListAnimeGenre(genre, page, perPage);
            if (animeListByGenre == null || !animeListByGenre.Any())
            {
                return NotFound();
            }

            var animeViewModels = animeListByGenre.Select(media => new AnimeViewModel
            {
                Id = media.Id,
                RomajiTitle = media.Title.Romaji,
                EnglishTitle = media.Title.English,
                NativeTitle = media.Title.Native,
                CoverImage = media.CoverImage.Large,
                Description = media.Description,
                Genres = media.Genres,
                Genre = genre,
                Score = media.Score,
                Episodes = media.Episodes,
                AverageScore = media.AverageScore,
            }).ToList();

            return PartialView("_AnimeListPartial", animeViewModels);
        }




    }

}
