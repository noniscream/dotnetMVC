using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Graphiczone.Controllers
{
    public class AdminController : Controller
    {

        public readonly GraphiczoneDBContext _graphiczoneDBContext;
        public AdminController(GraphiczoneDBContext graphiczoneDBContext)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var getsessionUsername = HttpContext.Session.GetString("AdminUsername");
                var searchData = _graphiczoneDBContext.User.Where(x => x.UserUsername == getsessionUsername).FirstOrDefault();
                if (searchData != null)
                {
                    ViewBag.UserUsername = HttpContext.Session.GetString("AdminUsername");
                    ViewBag.UserPosition = searchData.UserPosition;
                    ViewBag.UserStatus = searchData.UserStatus.ToString();
                }
                else
                {

                }
                //Response.Cookies.Append("LastLogedInTime", DateTime.Now.ToString());
            }

            var searchTotalincome = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus != null).Sum(y => y.OrPrintTotal);
            List<OrderPrint> searchCountTotal = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus != null).ToList();
            ViewBag.counttotal = searchCountTotal.Count();
            ViewBag.totalincome = searchTotalincome;
            var searchIncome = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus >= 2).Sum(y => y.OrPrintTotal);
            ViewBag.income = searchIncome;
            var searchCredit = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus < 2).Sum(y => y.OrPrintTotal);
            ViewBag.credit = searchCredit;
            return View();
        }

        public ViewResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public IActionResult LoginAuth(User user)
        {
            string username = user.UserUsername;
            string password = user.UserPassword;
            var seachData = _graphiczoneDBContext.User.Where(x => x.UserUsername == username && x.UserPassword == password && x.UserStatus == 1).FirstOrDefault();
            if (seachData != null)
            {
                HttpContext.Session.SetString("AdminUsername", user.UserUsername); // Save Session
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
    }
}
