using AnimeLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AnimeLibrary.Controllers
{
    public class CharacterController : Controller
    {
        private readonly AniListService _aniListService;

        public CharacterController(AniListService aniListService)
        {
            _aniListService = aniListService;
        }
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Details(int id)
        {
            // Получаем основные данные персонажа
            var character = await _aniListService.GetCharacterDetailsAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            // Получаем список медиа, где персонаж появляется
            var mediaAppearances = await _aniListService.GetCharacterAppearancesAsync(id);
            character.MediaAppearances = mediaAppearances;

            return View(character);
        }
    }
}
