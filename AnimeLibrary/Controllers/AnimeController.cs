using AnimeLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AnimeLibrary.Controllers
{
    public class AnimeController : Controller
    {
        private readonly AniListService _aniListService;

        public AnimeController(AniListService aniListService)
        {
            _aniListService = aniListService;
        }

        // Используйте асинхронный метод для получения данных
        public async Task<IActionResult> Details(int id)
        {
            // Асинхронно получаем данные об аниме
            var anime = await _aniListService.GetAnimeDetails(id);
            if (anime == null)
            {
                return NotFound();
            }
            var relatedAnime = await _aniListService.GetRelatedAnimeAsync(id);
            // Создаем ViewModel, используя данные аниме
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
                RelatedAnime = relatedAnime
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Genre(string genre)
        {
            // Асинхронно получаем список аниме по жанру
            var animeListByGenre = await _aniListService.ListAnimeGenre(genre);
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
                Score = media.Score,
                Episodes = media.Episodes,
                AverageScore = media.AverageScore
            }).ToList();

            // Получаем список всех жанров для выпадающего меню или других элементов интерфейса
            var allGenres = await _aniListService.GetGenresAsync();

            // Создаем HomeViewModel и инициализируем его свойства
            var viewModel = new HomeViewModel
            {
                // Инициализация списка аниме по жанру
                Action = genre == "Action" ? animeViewModels : new List<AnimeViewModel>(),
                AllAnimes = animeViewModels, // Убедитесь, что это свойство инициализировано правильно
                                             // ... инициализация других списков по жанрам
                Genres = allGenres ?? new List<string>(),
                // ... инициализация других свойств
            };

            return View("Genre", viewModel);
        }


        // КОД ДЛЯ ОТОБРАЖЕНИЯ ГОДОВ (НЕ ИСПОЛЬЗУЕТСЯ)
        // Добавьте новый метод в AnimeController
        /*public async Task<IActionResult> Year(int year)
        {
            // Асинхронно получаем список аниме по году выпуска
            var animeListByYear = await _aniListService.GetAnimeByYearAsync(year);
            if (animeListByYear == null || !animeListByYear.Any())
            {
                return NotFound();
            }

            // Создаем экземпляр HomeViewModel
            var homeViewModel = new HomeViewModel
            {
                // Здесь предполагается, что у HomeViewModel есть свойство AllAnimes
                AllAnimes = animeListByYear.Select(media => new AnimeViewModel
                {
                    Id = media.Id,
                    RomajiTitle = media.Title.Romaji,
                    EnglishTitle = media.Title.English,
                    NativeTitle = media.Title.Native,
                    CoverImage = media.CoverImage.Large,
                    Description = media.Description,
                    Genres = media.Genres,
                    Score = media.Score,
                    Episodes = media.Episodes,
                    AverageScore = media.AverageScore
                }).ToList()
                // Добавьте другие свойства, если они есть и требуются для представления
            };

            // Возвращаем представление с моделью HomeViewModel
            return View(homeViewModel);
        }
        */
        // Добавьте этот метод в класс AnimeController
        public async Task<IActionResult> Search(string title)
        {
            // Проверяем, не пуста ли строка поиска
            if (string.IsNullOrWhiteSpace(title))
            {
                // Возвращаем представление с сообщением о том, что поиск не может быть пустым
                return View("SearchResults", new List<AnimeViewModel>());
            }

            try
            {
                // Выполняем поиск аниме по названию
                var searchResults = await _aniListService.SearchAnimeByTitleAsync(title);
                var animeList = searchResults["Page"]["media"].ToObject<List<Media>>();

                
                // Преобразуем полученные данные в список AnimeViewModel
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

                // Возвращаем представление с результатами поиска
                return View("SearchResults", animeViewModels);
            }
            catch (Exception ex)
            {
                // Логируем исключение и возвращаем представление с ошибкой
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


    }

}
