using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

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

        public JsonResult LoginAuth(Customer cus)
        {
            string username = cus.CusUsername;
            string password = cus.CusPassword;
            var seachData = _graphiczoneDBContext.Customer.Where(x => x.CusUsername == username && x.CusPassword == password).FirstOrDefault();
            if(seachData != null)
            {
                HttpContext.Session.SetString("CusUsername",JsonConvert.SerializeObject(cus));
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public IActionResult Manager()
        {
                return View();
        }

        public ViewResult Register()
        {
            return View();
        }

        public JsonResult CheckUsernameAvilability(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var seachData = _graphiczoneDBContext.Customer.Where(x => x.CusUsername == userdata).FirstOrDefault();
            if (seachData != null)
            {
                return Json(seachData);
            }
            else
            {
                return Json(0);
            }
        }

        /*public JsonResult CheckFullnameAvilability(string fullnamedata)
        {
            System.Threading.Thread.Sleep(200);
            var seachFirstnameData = _graphiczoneDBContext.Customer.Where(x => x.CusFirstname == fullnamedata).FirstOrDefault();
            var seachLastnameData = _graphiczoneDBContext.Customer.Where(x => x.CusLastname == fullnamedata).FirstOrDefault();
            var full = seachFirstnameData + seachLastnameData.ToString();
            if(seachFirstnameData != null && seachLastnameData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }*/

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
            
            var genId = _graphiczoneDBContext.Customer.Count();
            customer.CusId = "GCUS00" + (genId + 1).ToString();
            
            _graphiczoneDBContext.Customer.Add(customer);
            _graphiczoneDBContext.SaveChanges();
            return Json("Success");
            
        }
    }
}
