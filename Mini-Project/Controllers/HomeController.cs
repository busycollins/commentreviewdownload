using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mini_Project.Logic;
using Mini_Project.Logic.Interface;
using Mini_Project.Models;

namespace Mini_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IYotubeService _yotubeService;
        private readonly IAmazonService _amazonService;
        public HomeController(ILogger<HomeController> logger, IYotubeService yotubeService,IAmazonService amazonService)
        {
            _logger = logger;
            _yotubeService = yotubeService;
            _amazonService = amazonService;

        }
        //IYotubeService _yotubeService;
        //public HomeController(IYotubeService yotubeService)
        //{
           
        //}
        public IActionResult Index()
        {
           
                return View();
        }
        [HttpPost]
        public ActionResult Subscribe(UrlModel model)
        {
            if (ModelState.IsValid)
            {//checks for the url 
                var checkYotubeUrl = _yotubeService.ExtractVideoIdFromUri(model.Url);
                if(checkYotubeUrl !=null)
                {
                    var csvString = _yotubeService.SearchReplies(model.Url);
                    var fileName = "YoutubeData " + DateTime.Now.ToString() + ".csv";
                    return File(new System.Text.UTF8Encoding().GetBytes(csvString), "text/csv", fileName);
                    //TODO: SubscribeUser(model.Email);
                }
              var text =   _amazonService.WebScrapper(model.Url.ToString());
                var fileString = "AmazonData " + DateTime.Now.ToString() + ".csv";
                return File(new System.Text.UTF8Encoding().GetBytes(text), "text/csv", fileString);
            }
           
            return View("Index", model);
        }
       
       
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SumbitUrl()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
