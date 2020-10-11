using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;

namespace Graphiczone.Controllers
{
    public class AdminController : Controller
    {

        public readonly GraphiczoneDBContext _graphiczoneDBContext;
        public AdminController(GraphiczoneDBContext graphiczoneDBContext)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
        }
        public IActionResult Index(int pageNumber = 1, int pageSize = 5)
        {
            DateTime dt = DateTime.Now;
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
            }
            GraphiczoneDBContext db = new GraphiczoneDBContext();
            var searchtotalorder = db.OrderPrint.Where(x => x.OrPrintStatus != null).ToList();
            ViewBag.totalorder = searchtotalorder.Count();
            var searchtotalworking = db.OrderPrint.Where(x => x.OrPrintStatus == 2).ToList();
            ViewBag.totalworking = searchtotalworking.Count();
            var searchtotalworkdone = db.OrderPrint.Where(x => x.OrPrintStatus == 3).ToList();
            ViewBag.totalworkdone = searchtotalworkdone.Count();
            var searchtotalshipping = db.OrderPrint.Where(x => x.OrPrintStatus == 4).ToList();
            ViewBag.totalshipping = searchtotalshipping.Count();
            var searchtotalincome = db.OrderPrint.Where(x => x.OrPrintStatus >= 0).Sum(x => x.OrPrintTotal);
            ViewBag.totalincome = searchtotalincome;
            var searchtotalpayment = db.OrderPrint.Where(x => x.OrPrintStatus >= 2).ToList();
            ViewBag.countpayment = searchtotalpayment.Count();
            ViewBag.totalpayment = searchtotalpayment.Sum(x => x.OrPrintTotal);
            var searchtotalcredit = db.OrderPrint.Where(x => x.OrPrintStatus < 2).ToList();
            ViewBag.countcredit = searchtotalcredit.Count();
            ViewBag.totalcredit = searchtotalcredit.Sum(x => x.OrPrintTotal);

            DateTime date = DateTime.Now;
            var searchCustomer = db.Customer.ToList();
            if (searchCustomer != null)
            {
                List<Customer> customers = searchCustomer.ToList();
                ViewBag.listCustomer = customers;
            }
            
            var serachEmployee = db.User.ToList();
            if(serachEmployee != null)
            {
                ViewBag.countemployee = serachEmployee.Count();
                ViewBag.totalemployee = serachEmployee.Sum(x => x.UserSalary);
            }


            int ExcludeRecords = (pageSize * pageNumber) - pageSize;
            var searchOrLatest = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus != null && x.OrPrintDate == date).OrderByDescending(x => x.OrPrintId)
                        .Skip(ExcludeRecords)
                        .Take(pageSize);
            if (searchOrLatest != null)
            {
                List<OrderPrint> orderPrints = searchOrLatest.ToList();
                ViewBag.listOrLatest = orderPrints;
                var searchPage = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus != null && x.OrPrintDate == date).Count();
                ViewBag.totalpage = searchPage;
                ViewBag.pagenumber = pageNumber;
                ViewBag.pagesize = pageSize;
                ViewBag.countOrLatest = searchPage;
            }
            else
            {
                ViewBag.listOrLatest = null;
            }
            
            return View(searchOrLatest);
        }

        public ViewResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUsername");
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
