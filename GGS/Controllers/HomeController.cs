using GGS.Models;
using GGS.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace GGS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private HomeService _homeService;

        public HomeController(IConfiguration configuration, HomeService homeService)
        {
            _configuration = configuration;
            _homeService = homeService;

        }


        public IActionResult Index()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult GetList(int UsId)
        {
            try
            {
                var lists = _homeService.GetList(UsId);
                return Result.FromData(Response, lists);
            }
            catch (Exception ex)
            {
                return Result.FromException(Response, ex);
            }
        }

        [HttpPost]
        public IActionResult GetGames()
        {
            try
            {
                var games = _homeService.GetGames();
                return Result.FromData(Response, games);
            }
            catch (Exception ex)
            {
                return Result.FromException(Response, ex);
            }
        }
    }
}
