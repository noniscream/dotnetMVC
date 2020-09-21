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
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Hosting;

namespace Graphiczone.Controllers
{
    public class HomeController : Controller
    {
        private readonly GraphiczoneDBContext _graphiczoneDBContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, GraphiczoneDBContext graphiczoneDBContext, IWebHostEnvironment hostEnvironment)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
            this._webHostEnvironment = hostEnvironment;
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
        public IActionResult Tracking(string id)
        {
            if (id != null)
            {
                var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id.Trim()).FirstOrDefault();
                if(searchData != null)
                {
                    ViewBag.OrderPrintId = searchData.OrPrintId;
                    var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == searchData.CusId).FirstOrDefault();
                    ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                    ViewBag.OrderDate = searchData.OrPrintDate.Value.ToString("dd MMMM yyyy");
                    ViewBag.OrderDue = searchData.OrPrintDue.Value.ToString("dd MMMM yyyy");
                    var prf = _graphiczoneDBContext.ProofPayment.Where(p => p.OrPrintId == searchData.OrPrintId).FirstOrDefault();
                    ViewBag.PrfDate = prf.PrfPayDate;
                    ViewBag.Orderstatus = searchData.OrPrintStatus;
                    return View(searchData);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
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
