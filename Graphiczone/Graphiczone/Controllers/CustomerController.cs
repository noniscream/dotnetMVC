using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Graphiczone.Controllers
{
    public class CustomerController : Controller
    {
        private readonly GraphiczoneDBContext _graphiczoneDBContext;

        public CustomerController(GraphiczoneDBContext graphiczoneDBContext)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ViewResult Login()
        {
            return View();
        }

        public ViewResult Register()
        {
            return View();
        }

        public ViewResult MSSQLSERVER()
        {
            var data = _graphiczoneDBContext.Customer.ToList();
            return View(data);
        }

        public JsonResult CheckUsernameAvilability(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var seachData = _graphiczoneDBContext.Customer.Where(x => x.CusId == userdata).FirstOrDefault();
            if (seachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckTelAvilability(string teldata)
        {
            System.Threading.Thread.Sleep(200);
            var seachData = _graphiczoneDBContext.Customer.Where(x => x.CusTel == teldata).FirstOrDefault();
            if (seachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckEmailAvilability(string emaildata)
        {
            System.Threading.Thread.Sleep(200);
            var seachData = _graphiczoneDBContext.Customer.Where(x => x.CusEmail == emaildata).FirstOrDefault();
            if (seachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpPost]
        public JsonResult Create(Customer customer)
        {
            _graphiczoneDBContext.Customer.Add(customer);
            _graphiczoneDBContext.SaveChanges();
            return Json("Success");
        }
    }
}
