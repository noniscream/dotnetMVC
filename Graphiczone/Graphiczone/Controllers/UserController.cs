using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Graphiczone.Controllers
{
    public class UserController : Controller
    {
        DateTime dt = DateTime.Now;

        private readonly GraphiczoneDBContext _graphiczoneDBContext;

        public UserController(GraphiczoneDBContext graphiczoneDBContext)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var getsessionUsername = HttpContext.Session.GetString("UserUsername");
                var seachData = _graphiczoneDBContext.User.Where(x => x.UserUsername == getsessionUsername).FirstOrDefault();
                if(seachData != null)
                {
                    ViewBag.UserUsername = HttpContext.Session.GetString("UserUsername");
                    ViewBag.UserPosition = seachData.UserPosition;
                    ViewBag.UserStatus = seachData.UserStatus.ToString();

                }
                else
                {

                }
            }
            return View();
        }
        public ViewResult Login()
        {
            
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserUsername");
            return RedirectToAction("Login");
        }
        public JsonResult LoginAuth(User user)
        {
            string username = user.UserUsername;
            string password = user.UserPassword;
            var seachData = _graphiczoneDBContext.User.Where(x => x.UserUsername == username && x.UserPassword == password).FirstOrDefault();
            if (seachData != null)
            {
                HttpContext.Session.SetString("UserUsername", user.UserUsername); // Save Session
                HttpContext.Session.SetString("forlistreport", "0");
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public IActionResult ListWorkStatus()
        {
            var seachData = _graphiczoneDBContext.OrderPrint.ToList();
            return View(seachData);
        }
        
        
    }
}
