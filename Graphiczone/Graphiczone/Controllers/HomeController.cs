using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Graphiczone.Models;
using System.Net.Mail;
using System.Net;

namespace Graphiczone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ViewResult Service()
        {
            return View();
        }

        public ViewResult Order()
        {
            return View();
        }

        public ViewResult Payment()
        {
            return View();
        }

        public ViewResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }
        public ViewResult Tracking()
        {
            return View();
        }
        public ViewResult Inkjet()
        {
            return View();
        }
        public ViewResult ChannelLetter()
        {
            return View();
        }
        public ViewResult Lightbox()
        {
            return View();
        }
        public ViewResult LED()
        {
            return View();
        }
        public ViewResult Tower()
        {
            return View();
        }
        public ViewResult Sticker()
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
