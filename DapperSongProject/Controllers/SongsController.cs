using DapperSongProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DapperSongProject.Controllers
{
    public class SongsController : Controller
    {
        private readonly ISongRepository _songRepository;

        public SongsController(ISongRepository songRepository)
        {
            _songRepository = songRepository;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? q = null)
        {
            var model = await _songRepository.GetSongsPagedAsync(page, pageSize, q);
            return View(model);
        }
    }
}
