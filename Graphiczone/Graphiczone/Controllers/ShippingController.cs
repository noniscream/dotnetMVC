using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Graphiczone.Controllers
{
    public class ShippingController : Controller
    {

        private readonly GraphiczoneDBContext _graphiczoneDBContext;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ShippingController(GraphiczoneDBContext graphiczoneDBContext, IWebHostEnvironment hostEnvironment)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
            this._webHostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListWorkShipping(string id)
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return View("../User/Login");
            }
            else
            {
                if (id != null)
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 3 && x.OrPrintId == id).ToList();
                    ViewBag.countData = searchData.Count();
                    return View(searchData);
                }
                else
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 3 && id == null).ToList();
                    ViewBag.countData = searchData.Count();
                    return View(searchData);
                }
            }
        }
        public IActionResult EditWorkShipping(string id = "0")
        {
            if (id == "0")
            {
                return View();
            }
            else
            {

                var seachData = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
                var seachData2 = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
                if (seachData2 != null)
                {
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).ToList();
                    ViewBag.OrderList = orderDetailPrints;
                    ViewBag.OrderPrintId = seachData.OrPrintId;
                    ViewBag.OrderDate = seachData2.OrPrintDate;
                    ViewBag.GetDateForPicker = seachData2.OrPrintDate.Value.AddYears(-543);
                    var searchDataProofpay = _graphiczoneDBContext.ProofPayment.Where(x => x.OrPrintId == id).FirstOrDefault();
                    if (searchDataProofpay != null)
                    {
                        ViewBag.getDatePayForPicker = searchDataProofpay.PrfPayDate.Value.AddYears(-543).ToString("yyyy-MM-dd");
                    }
                    ViewBag.getDueForPicker = seachData2.OrPrintDue.Value.AddMonths(+2).AddYears(-543).ToString("yyyy-MM-dd");
                    //ViewBag.GetDueForPicker = seachData2.OrPrintDue.Value.AddYears(-543);
                    ViewBag.OrderDue = seachData2.OrPrintDue;
                    var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                    ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                    ViewBag.TotalDate = seachData2.OrPrintDue - DateTime.Now;
                    var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == seachData.PrintId).FirstOrDefault();
                    ViewBag.PrintName = printname.PrintName;
                    ViewBag.PriceTotal = seachData2.OrPrintTotal;
                }
                return PartialView("_EditWorkShipping", seachData);
            }
        }

        [HttpPost]
        public async Task<JsonResult> updateAsync(OrderPrint orderPrint, Shipping shipping)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;

                shipping.OrPrintId = orderPrint.OrPrintId;
                shipping.ShippingDate = shipping.ShippingDate.Value.AddYears(543);
                var sessionid = HttpContext.Session.GetString("UserUsername");
                var getUserid = _graphiczoneDBContext.User.Where(x => x.UserUsername == sessionid).FirstOrDefault();
                shipping.UserId = getUserid.UserId;

                
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(shipping.ShippingFile);
                string extension = Path.GetExtension(shipping.ShippingFile);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                shipping.ShippingFile = "images/" + fileName;
                fileName = Path.Combine(wwwRootPath,"images", fileName);


                using(var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    
                }

                _graphiczoneDBContext.Shipping.Add(shipping);
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }


        }
        public IFormFile Uploadfile { get; set; }

        [HttpPost]
        public async Task<IActionResult> uploadAsync(OrderPrint orderPrint, Shipping shipping)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();

            if(searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;

                shipping.OrPrintId = orderPrint.OrPrintId;
                shipping.ShippingDate = shipping.ShippingDate.Value.AddYears(543);
                var sessionid = HttpContext.Session.GetString("UserUsername");
                var getUserid = _graphiczoneDBContext.User.Where(x => x.UserUsername == sessionid).FirstOrDefault();
                shipping.UserId = getUserid.UserId;

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(Uploadfile.FileName);
                string extension = Path.GetExtension(Uploadfile.FileName);
                
                if(extension == ".jpg" || extension==".png" || extension == ".gif")
                {
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    shipping.ShippingFile = "images/" + fileName;
                    fileName = Path.Combine(wwwRootPath, "images", fileName);


                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await Uploadfile.CopyToAsync(fileStream);
                    }
                }

                _graphiczoneDBContext.Shipping.Add(shipping);
                _graphiczoneDBContext.SaveChanges();
                var x = "คุณได้ทำการบันทึกการส่งมอบ รายการที่ : "+orderPrint.OrPrintId+ "เรียบร้อยแล้ว";
                return RedirectToAction("ListWorkShipping");
            }

            return Json("ไม่สำเร็จ");
        }

    }
}
