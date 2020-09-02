using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Graphiczone.Models;
using Graphiczone.Models.SQLServer;

namespace Graphiczone.Controllers
{
    public class OrderDetailPrintController : Controller
    {
        private readonly GraphiczoneDBContext _graphiczoneDBContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderDetailPrintController(GraphiczoneDBContext graphiczoneDBContext, IWebHostEnvironment hostEnvironment)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
            this._webHostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult CreateOrder()
        {
            if (HttpContext.Session.GetString("CusUsername") == null)
            {

            }
            else
            {
                ViewBag.CusUsername = HttpContext.Session.GetString("CusUsername");
                //Response.Cookies.Append("LastLogedInTime", DateTime.Now.ToString());
            }
            var sessionid = HttpContext.Session.GetString("CusUsername");
            var getCusId = _graphiczoneDBContext.Customer.Where(x => x.CusUsername == sessionid).FirstOrDefault();

            if (getCusId != null)
            {
                ViewBag.Cusid = getCusId.CusId;
                ViewBag.CusFirstname = getCusId.CusFirstname + " " + getCusId.CusLastname;
                ViewBag.CusTel = getCusId.CusTel;
                ViewBag.CusAddress = getCusId.CusAddress;
                ViewBag.Date = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            }
            else
            {

            }

            var getorprintid = HttpContext.Session.GetString("Cart");
            var getOrPrintDetail = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == getorprintid).ToList();
            var searchOrderPrint = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == getorprintid).FirstOrDefault();
            if (searchOrderPrint != null)
            {
                var printname = _graphiczoneDBContext.Print.ToList();
                List<Print> prints = printname.ToList();
                ViewBag.printname = prints;
                ViewBag.OrPrintDate = searchOrderPrint.OrPrintDate;
            }
            List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
            ViewBag.typeprint = _graphiczoneDBContext.TypePrint.ToList();
            var print = _graphiczoneDBContext.Print.FirstOrDefault();
            //var search = _graphiczoneDBContext.Print.Where(x => x.PrintId == orderDetailPrint.PrintId).FirstOrDefault();
            //ViewBag.printprice = search.PrintPrice;
            return View(getOrPrintDetail);
        }

        public JsonResult getselectbyid(string id)
        {
            List<Print> list = new List<Print>();
            list = _graphiczoneDBContext.Print.Where(x => x.TypePrint.TypePrintId == id).ToList();
            //list.Insert(0, new Print { Id = 0, PrintId = "P000", PrintName = "กรุณาเลือกประเภทวัสดุ" });
            return Json(new SelectList(list, "PrintId", "PrintName"));
        }
        public IFormFile Uploadfile { get; set; }

        [HttpPost]
        public async Task<IActionResult> uploadAsync(OrderPrint orderPrint, OrderDetailPrint orderDetailPrint, Print print, TypePrint typePrint)
        {
            orderDetailPrint.PrintId = orderDetailPrint.PrintId.Trim();
            //var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            //if (searchData != null)
            //{
            //searchData.OrPrintStatus = orderPrint.OrPrintStatus;
            //orderDetailPrint.OrPrintId = orderPrint.OrPrintId;
            //var sessionid = HttpContext.Session.GetString("UserUsername");
            //var getUserid = _graphiczoneDBContext.User.Where(x => x.UserUsername == sessionid).FirstOrDefault();
            //shipping.UserId = getUserid.UserId;
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(Uploadfile.FileName);
            string extension = Path.GetExtension(Uploadfile.FileName);

            if (extension == ".jpg" || extension == ".png" || extension == ".gif")
            {
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                orderDetailPrint.OrdPrintFile = "images/" + fileName;
                fileName = Path.Combine(wwwRootPath, "images", fileName);

                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    await Uploadfile.CopyToAsync(fileStream);
                }
            }
            var printid = _graphiczoneDBContext.Print.Where(x => x.PrintId == orderDetailPrint.PrintId).FirstOrDefault();
            orderDetailPrint.OrdPrintPriceset = printid.PrintPrice;
            int area = (int)(orderDetailPrint.OrdPrintHeight * orderDetailPrint.OrdPrintWidth);
            float printtotal = (float)(area * orderDetailPrint.OrdPrintPriceset * orderDetailPrint.OrdPrintNum);
            orderDetailPrint.OrdPrintTotal = printtotal;
            var genId = _graphiczoneDBContext.OrderDetailPrint.Count();
            orderDetailPrint.OrdPrintId = DateTime.Now.ToString("yyMMddHHmmssf") + "0" + (genId + 1).ToString();
            orderDetailPrint.OrPrintId = HttpContext.Session.GetString("Cart"); // เพิ่ม
            _graphiczoneDBContext.OrderDetailPrint.Add(orderDetailPrint);
            _graphiczoneDBContext.SaveChanges();

            //var x = "คุณได้ทำการบันทึกการส่งมอบ รายการที่ : " + orderPrint.OrPrintId + "เรียบร้อยแล้ว";
            return RedirectToAction("CreateOrder");
            //}

            //return Json("ไม่สำเร็จ");
        }

        [HttpPost]
        public JsonResult deleteorder(OrderDetailPrint orderDetailPrint)
        {
            var searchData = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrdPrintId == orderDetailPrint.OrdPrintId).FirstOrDefault();
            if (searchData != null)
            {
                _graphiczoneDBContext.Remove(searchData);
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        // บรีสยกเลิก
        [HttpPost]
        public JsonResult clearsession(OrderPrint orderPrint, OrderDetailPrint orderDetailPrint)
        {
            var searchOrPrintId = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchOrPrintId != null)
            {
                var searchOrdPrintId = _graphiczoneDBContext.OrderDetailPrint.Where(y => y.OrPrintId == searchOrPrintId.OrPrintId).FirstOrDefault();
                if (searchOrdPrintId != null)
                {
                    var countOrdPrintId = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == searchOrdPrintId.OrPrintId).ToList();
                    for (int i = 1; i <= countOrdPrintId.Count; i++)
                    {
                        var searchdataforclear = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == searchOrPrintId.OrPrintId).FirstOrDefault();
                        _graphiczoneDBContext.Remove(searchdataforclear);
                        _graphiczoneDBContext.SaveChanges();
                    }
                }
                _graphiczoneDBContext.Remove(searchOrPrintId);
                HttpContext.Session.Remove("Cart");
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        // บรีสยกเลิก
        [HttpPost]
        public IActionResult opbill(OrderPrint orderPrint, Customer customer)
        {
            var searchCus = _graphiczoneDBContext.Customer.Where(x => x.CusId == customer.CusId).FirstOrDefault();
            if (searchCus != null)
            {
                var maxbill = _graphiczoneDBContext.OrderPrint.Count();
                var getid = orderPrint.OrPrintId = DateTime.Now.ToString("yyMMddHHmmssf") + (maxbill + 1).ToString();
                orderPrint.OrPrintId = getid;
                orderPrint.CusId = searchCus.CusId;
                orderPrint.OrPrintDate = orderPrint.OrPrintDate;
                var getorprintdate = orderPrint.OrPrintDate;
                HttpContext.Session.SetString("Cart", getid);
                HttpContext.Session.SetString("OrPrintDate", getorprintdate.ToString());
                _graphiczoneDBContext.Add(orderPrint);
                _graphiczoneDBContext.SaveChanges();
                return Json(getid);
            }
            else
            {
                return Json(0);
            }

        }

        public IActionResult successbill(OrderDetailPrint orderDetailPrint, OrderPrint orderPrint)
        {
            var searchBill = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchBill != null)
            {
                orderPrint.OrPrintDate = DateTime.Now;
                searchBill.OrPrintDue = orderPrint.OrPrintDue;
                searchBill.OrPrintDate = orderPrint.OrPrintDate;
                searchBill.OrPrintStatus = orderPrint.OrPrintStatus;
                searchBill.OrPrintTotal = orderPrint.OrPrintTotal;
                _graphiczoneDBContext.SaveChanges();
                HttpContext.Session.Remove("Cart");
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
    }
}


