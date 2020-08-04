using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graphiczone.Models;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graphiczone.Controllers
{
    public class OrderPrintController : Controller
    {
        private readonly GraphiczoneDBContext _graphiczoneDBContext;

        public OrderPrintController(GraphiczoneDBContext graphiczoneDBContext)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return RedirectToAction("");
            }
            else
            {
                return View();
            }

        }

        public IActionResult ListWorkStatus()
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return RedirectToAction("");
            }
            else
            {
                var seachData = _graphiczoneDBContext.OrderPrint.Where(x=>x.OrPrintStatus == 1).ToList();
                return View(seachData);
            }
        }

        [HttpGet]
        public IActionResult Edit(string id = "0")
        {
            if (id == "0")
            {
                return View();
            }
            else
            {
                
                var seachData = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
                var seachData2 = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
                if(seachData2 != null)
                {
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id ).ToList();
                    ViewBag.OrderList = orderDetailPrints;
                    ViewBag.OrderPrintId = seachData.OrPrintId;
                    ViewBag.OrderDate = seachData2.OrPrintDate;
                    ViewBag.OrderDue = seachData2.OrPrintDue;
                    var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                    ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                    ViewBag.TotalDate = seachData2.OrPrintDue - DateTime.Now;
                    var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == seachData.PrintId).FirstOrDefault();
                    ViewBag.PrintName = printname.PrintName;
                    ViewBag.PriceTotal = seachData2.OrPrintTotal;
                }
                return PartialView("_EditWorkStatus",seachData);
            }
        }
        [HttpPost]
        public JsonResult update(OrderPrint orderPrint)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if( searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
            
            
        }
    }
}
