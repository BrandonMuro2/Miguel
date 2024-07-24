using GGS.Models;
using GGS.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Formats.Tar;

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


        [HttpGet]
        public IActionResult GetSignalsBySubject(int subjectId, int channelId, string type)
        {
            try
            {
                var lists = _homeService.GetSignalsBySubject(subjectId, channelId, type);
                return Result.FromData(Response, lists);
            }
            catch (Exception ex)
            {
                return Result.FromException(Response, ex);
            }
        }

        [HttpGet]
        public IActionResult GetSubjects()
        {
            try
            {
                var lists = _homeService.GetSubjects();
                return Result.FromData(Response, lists);
            }
            catch (Exception ex)
            {
                return Result.FromException(Response, ex);
            }
        }

        [HttpGet]
        public IActionResult GetChannels()
        {
            try
            {
                var lists = _homeService.GetChannels();
                return Result.FromData(Response, lists);
            }
            catch (Exception ex)
            {
                return Result.FromException(Response, ex);
            }
        }
        [HttpGet]
        public IActionResult GetActivityByChannels(string channelNames)
        {
            try
            {
                var lists = _homeService.GetActivityByChannels(channelNames);
                return Result.FromData(Response, lists);
            }
            catch (Exception ex)
            {
                return Result.FromException(Response, ex);
            }
        }

        //[HttpPost]
        //public IActionResult InsertBackgroundAndTask(IFormFile backgroundfile, IFormFile taskfile)
        //{
        //    try
        //    {
        //        _homeService.InsertBackgroundAndTask(backgroundfile, taskfile);
        //        return Result.FromCreated(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result.FromException(Response, ex);
        //    }
        //}



    }
}
