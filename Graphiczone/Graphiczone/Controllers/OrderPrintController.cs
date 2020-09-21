using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages;
using Graphiczone.Models;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ReflectionIT.Mvc.Paging;

namespace Graphiczone.Controllers
{
    public class OrderPrintController : Controller
    {

        DateTime dt = DateTime.Now;

        private readonly GraphiczoneDBContext _graphiczoneDBContext;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public OrderPrintController(GraphiczoneDBContext graphiczoneDBContext, IWebHostEnvironment webHostEnvironment)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
            _webHostEnvironment = webHostEnvironment;
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

        public IActionResult ListWorkAll(string id)
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return View("../User/Login");
            }
            else
            {
                if (id != null)
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus != null && x.OrPrintId == id ).OrderByDescending(x=>x.OrPrintStatus == 0).ToList();
                    ViewBag.countData = searchData.Count();
                    return View(searchData);
                }
                else
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus != null && id == null ).OrderByDescending(x => x.OrPrintStatus == 0);
                    ViewBag.countData = searchData.Count();
                    return View(searchData);
                }
            }
        }

        public IActionResult ListOrderBin()
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return View("../User/Login");
            }
            else
            {
                var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == null || x.OrPrintStatus == 0 && x.OrPrintDue <= dt).OrderByDescending(x => x.OrPrintStatus == 0).ToList();
                ViewBag.countData = searchData.Count();
                return View(searchData);
            }
        }

        public IActionResult ConfirmShipping(string id = "0")
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
                    ViewBag.OrStatus = seachData2.OrPrintStatus;
                    ViewBag.OrderDate = seachData2.OrPrintDate;
                    ViewBag.OrderDue = seachData2.OrPrintDue;
                    var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                    ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                    var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == seachData.PrintId).FirstOrDefault();
                    ViewBag.PrintName = printname.PrintName;
                    ViewBag.PriceTotal = seachData2.OrPrintTotal;
                    var searchShipping = _graphiczoneDBContext.Shipping.Where(x => x.OrPrintId == id).FirstOrDefault();
                    if(searchShipping != null)
                    {
                        ViewBag.dateShipping = searchShipping.ShippingDate;
                        ViewBag.picShipping = searchShipping.ShippingFile;
                    }
                    else
                    {
                        ViewBag.dateShipping = null;
                        ViewBag.picShipping = null;
                    }
                }
                return PartialView("_ConfirmShipping", seachData);
            }
        }

        [HttpPost]
        public JsonResult updateshipping(OrderPrint orderPrint)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
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

        [HttpPost]
        public JsonResult deleteorderbin(OrderPrint orderPrint)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
            {
                var searchlist = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == searchData.OrPrintId).ToList();
                if(searchlist != null)
                {
                    for(int i = 1; i <= searchlist.Count; i++)
                    {
                        var searchorderdetail = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == searchData.OrPrintId).FirstOrDefault();
                        _graphiczoneDBContext.Remove(searchorderdetail);
                        _graphiczoneDBContext.SaveChanges();
                    }
                }
                _graphiczoneDBContext.Remove(orderPrint);
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpGet]
        public IActionResult GetWorkDetail(string id)
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return View("../User/Login");
            }
            else
            {
                if (id == "0")
                {
                    return View();
                }
                else
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
                    if (searchData != null)
                    {
                        ViewBag.OrPrintId = searchData.OrPrintId;
                        var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == searchData.CusId).FirstOrDefault();
                        ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                        ViewBag.CusAddress = cusname.CusAddress;
                        ViewBag.CusTel = cusname.CusTel;
                        ViewBag.CusEmail = cusname.CusEmail;
                        ViewBag.OrPrintDate = searchData.OrPrintDate;
                        ViewBag.OrPrintDue = searchData.OrPrintDue;

                        var searchDataProofpay = _graphiczoneDBContext.ProofPayment.Where(x => x.OrPrintId == id).FirstOrDefault();
                        if (searchDataProofpay != null)
                        {
                            ViewBag.getDatePayForPicker = searchDataProofpay.PrfPayDate.Value.AddYears(-543).ToString("yyyy-MM-dd");
                        }
                        ViewBag.getDueForPicker = searchData.OrPrintDue.Value.AddMonths(+2).AddYears(-543).ToString("yyyy-MM-dd");

                        var calOrPrintData = searchData.OrPrintDue - DateTime.Now;
                        ViewBag.OrPrintTotalDate = calOrPrintData.Value.ToString("dd");
                        var searchDataOrDetail = _graphiczoneDBContext.OrderDetailPrint.Where(ord => ord.OrPrintId == id).FirstOrDefault();
                        if (searchDataOrDetail != null)
                        {
                            List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.Where(ord => ord.OrPrintId == id).ToList();
                            ViewBag.OrPrintDetail = orderDetailPrints;
                            var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == searchDataOrDetail.PrintId).FirstOrDefault();
                            ViewBag.PrintName = printname.PrintName;
                            ViewBag.PriceTotal = searchData.OrPrintTotal;
                        }
                        var searchDataProofpayment = _graphiczoneDBContext.ProofPayment.Where(x => x.OrPrintId == id).FirstOrDefault();
                        if (searchDataProofpayment != null)
                        {
                            ViewBag.getPayCode = searchDataProofpayment.PrfPayId;
                            ViewBag.getPayBank = searchDataProofpayment.PrfPayBank;
                            ViewBag.getPayDate = searchDataProofpayment.PrfPayDate.Value.ToString("dd/MM/yyyy");
                            ViewBag.getPayTime = searchDataProofpayment.PrfPayTime;
                            ViewBag.getImgCode = searchDataProofpayment.PrfPayFile;
                            ViewBag.getPayDetail = searchDataProofpayment.PrfPayDetail;
                        }
                        else
                        {
                            ViewBag.getPayDate = "";
                        }

                        var searchDataShipping = _graphiczoneDBContext.Shipping.Where(x => x.OrPrintId == id).FirstOrDefault();
                        if (searchDataShipping != null)
                        {
                            ViewBag.getShippingDate = searchDataShipping.ShippingDate.Value.AddYears(-543).ToString("yyyy-MM-dd");
                            ViewBag.getShippingImg = searchDataShipping.ShippingFile;
                        }
                        else
                        {
                            ViewBag.getShippingDate = "";
                        }
                        return View("GetWorkDetail", searchData);
                    }
                    else
                    {
                        ViewBag.OrPrintId = "xx";
                    }
                }
            }
            return View();

        }

        [HttpPost]
        public JsonResult updatepayment(OrderPrint orderPrint)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
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


        // เฟม

        public IActionResult ConfirmPayment(String id)
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return View("../User/Login");
            }
            else
            {
                if(id != null)
                {
                    var seachData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 1 && x.OrPrintId == id).ToList();
                    ViewBag.countData = seachData.Count();
                    return View(seachData);
                }
                else
                {
                    var seachData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 1 && x.OrPrintId != null).ToList();
                    ViewBag.countData = seachData.Count();
                    return View(seachData);
                }
                
            }

        }

        public IActionResult ListConfirmPayment(string id)
        {
            if (HttpContext.Session.GetString("CusUsername") == null)
            {
                return View("../Customer/Login");
            }
            else
            {
                var sessionid = HttpContext.Session.GetString("CusUsername");
                var getCusId = _graphiczoneDBContext.Customer.Where(x => x.CusUsername == sessionid).FirstOrDefault();
                var cusId = getCusId.CusId;
                if (id != null)
                {

                    var seachData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 2 && x.CusId == cusId && x.OrPrintId == id).OrderByDescending(x => x.OrPrintId).ToList();
                    return View(seachData);
                }
                else
                {
                    var seachData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 2 && x.CusId == cusId && id == null).OrderByDescending(x => x.OrPrintId).ToList();
                    return View(seachData);
                }
            }


        }

        public IActionResult ListCustomerWorkStatus(string id)
        {
            if (HttpContext.Session.GetString("CusUsername") == null)
            {
                return View("../Customer/Login");
            }
            else
            {
                var sessionid = HttpContext.Session.GetString("CusUsername");
                var getCusId = _graphiczoneDBContext.Customer.Where(x => x.CusUsername == sessionid).FirstOrDefault();
                var cusId = getCusId.CusId;
                if (id != null)
                {

                    var seachData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus >= 2 && x.OrPrintStatus <= 3 && x.CusId == cusId && x.OrPrintId == id).OrderByDescending(x => x.OrPrintId).ToList();
                    return View(seachData);
                }
                else
                {
                    var seachData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus >= 2 && x.OrPrintStatus <= 3 && x.CusId == cusId && id == null).OrderByDescending(x => x.OrPrintId).ToList();
                    return View(seachData);
                }
            }


        }

        public IActionResult ListCustomerWorkShipping(string id)
        {
            if (HttpContext.Session.GetString("CusUsername") == null)
            {
                return View("../Customer/Login");
            }
            else
            {
                var sessionid = HttpContext.Session.GetString("CusUsername");
                var getCusId = _graphiczoneDBContext.Customer.Where(x => x.CusUsername == sessionid).FirstOrDefault();
                var cusId = getCusId.CusId;
                if (id != null)
                {

                    var seachData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 4 && x.CusId == cusId && x.OrPrintId == id).OrderByDescending(x => x.OrPrintId).ToList();
                    return View(seachData);
                }
                else
                {
                    var seachData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 4 && x.CusId == cusId && id == null).OrderByDescending(x => x.OrPrintId).ToList();
                    return View(seachData);
                }
            }


        }

        public IActionResult ShowOrder(string id)
        {
            if (HttpContext.Session.GetString("CusUsername") == null)
            {
                return View("../Customer/Login");
            }
            else
            {
                var sessionid = HttpContext.Session.GetString("CusUsername");
                var getCusId = _graphiczoneDBContext.Customer.Where(x => x.CusUsername == sessionid).FirstOrDefault();
                var cusId = getCusId.CusId;
                if (id != null)
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.CusId == cusId && x.OrPrintId == id && x.OrPrintStatus != null  || x.OrPrintStatus == null && x.OrPrintDue != null ).OrderByDescending(x => x.OrPrintId).ToList();
                    return View(searchData);
                }
                else
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.CusId == cusId && id == null && x.OrPrintStatus != null  || x.OrPrintStatus == null && x.OrPrintDue != null ).OrderByDescending(x => x.OrPrintId).ToList();
                    return View(searchData);
                }
            }
        }

        [HttpGet]
        public IActionResult ShowOrderDetail(string id)
        {
            var seachData = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
            var seachData2 = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
            if (seachData2 != null)
            {
                List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).ToList();
                ViewBag.OrderList = orderDetailPrints;
                ViewBag.OrderPrintId = seachData.OrPrintId;
                ViewBag.OrderDate = seachData2.OrPrintDate;
                ViewBag.OrderDue = seachData2.OrPrintDue;
                var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                ViewBag.TotalDate = seachData2.OrPrintDue - DateTime.Now;
                var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == seachData.PrintId).FirstOrDefault();
                var prooffile = _graphiczoneDBContext.ProofPayment.Where(a => a.OrPrintId == seachData2.OrPrintId).FirstOrDefault();
                ViewBag.PrintName = printname.PrintName;
                ViewBag.PriceTotal = seachData2.OrPrintTotal;
                ViewBag.OrStatus = seachData2.OrPrintStatus;
            }

            return PartialView("_ShowOrderDetail", seachData);
        }

        [HttpGet]
        public IActionResult ListConfirmPaymentDetail(string id)
        {
            var seachData = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
            var seachData2 = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
            if (seachData2 != null)
            {
                List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).ToList();
                ViewBag.OrderList = orderDetailPrints;
                ViewBag.OrderPrintId = seachData.OrPrintId;
                ViewBag.OrderDate = seachData2.OrPrintDate;
                ViewBag.OrderDue = seachData2.OrPrintDue;
                var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                ViewBag.TotalDate = seachData2.OrPrintDue - DateTime.Now;
                var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == seachData.PrintId).FirstOrDefault();
                var prooffile = _graphiczoneDBContext.ProofPayment.Where(a => a.OrPrintId == seachData2.OrPrintId).FirstOrDefault();
                ViewBag.PrintName = printname.PrintName;
                ViewBag.PriceTotal = seachData2.OrPrintTotal;
                ViewBag.ProofFile = prooffile.PrfPayFile;
            }

            return PartialView("_ListConfirmPaymentDetail", seachData);
        }

        [HttpGet]
        public IActionResult EditConfirmPayment(string id)
        {
            var seachData = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
            var seachData2 = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
            if (seachData2 != null)
            {
                List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).ToList();
                ViewBag.OrderList = orderDetailPrints;
                ViewBag.OrderPrintId = seachData.OrPrintId;
                ViewBag.OrderDate = seachData2.OrPrintDate;
                ViewBag.OrderDue = seachData2.OrPrintDue;
                var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                ViewBag.TotalDate = seachData2.OrPrintDue - DateTime.Now;
                var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == seachData.PrintId).FirstOrDefault();
                var prooffile = _graphiczoneDBContext.ProofPayment.Where(a => a.OrPrintId == seachData2.OrPrintId).FirstOrDefault();
                ViewBag.PrintName = printname.PrintName;
                ViewBag.PriceTotal = seachData2.OrPrintTotal;
                ViewBag.ProofFile = prooffile.PrfPayFile;
            }

            return PartialView("_EditConfirmPayment", seachData);
        }

        [HttpPost]
        public JsonResult updateconfirmpayment(OrderPrint orderPrint)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
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

        [HttpPost]
        public JsonResult deleteconfirmpayment(OrderPrint orderPrint, Shipping shipping, ProofPayment proofPayment)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            var searchData2 = _graphiczoneDBContext.Shipping.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            var searchData3 = _graphiczoneDBContext.ProofPayment.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;
                if (searchData2 != null)
                {
                    _graphiczoneDBContext.Remove(searchData2);
                }
                if (searchData3 != null)
                {
                    _graphiczoneDBContext.Remove(searchData3);
                }
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        // เฟม

        [HttpPost]
        public JsonResult updateworkstatus(OrderPrint orderPrint)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
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

        public IFormFile Uploadfile { get; set; }

        [HttpPost]
        public async Task<IActionResult> uploadAsync(OrderPrint orderPrint, Shipping shipping)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            var getname = "";
            if (searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;

                shipping.OrPrintId = orderPrint.OrPrintId;
                shipping.ShippingDate = shipping.ShippingDate.Value.AddYears(543);
                var sessionid = HttpContext.Session.GetString("UserUsername");
                var getUserid = _graphiczoneDBContext.User.Where(x => x.UserUsername == sessionid).FirstOrDefault();
                if (getUserid != null)
                {
                    shipping.UserId = getUserid.UserId;
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if(Uploadfile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Uploadfile.FileName);
                    string extension = Path.GetExtension(Uploadfile.FileName);

                    if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                    {
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        shipping.ShippingFile = "images/" + fileName;
                        getname = "images/" + fileName;
                        fileName = Path.Combine(wwwRootPath, "images", fileName);


                        using (var fileStream = new FileStream(fileName, FileMode.Create))
                        {
                            await Uploadfile.CopyToAsync(fileStream);
                        }
                        var searchDataShipping = _graphiczoneDBContext.Shipping.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
                        if (searchDataShipping != null)
                        {
                            searchDataShipping.OrPrintId = orderPrint.OrPrintId;
                            searchDataShipping.ShippingDate = shipping.ShippingDate;
                            searchDataShipping.ShippingFile = getname;
                            searchDataShipping.UserId = getUserid.UserId;
                            _graphiczoneDBContext.SaveChanges();
                        }
                        else
                        {
                            _graphiczoneDBContext.Shipping.Add(shipping);
                            _graphiczoneDBContext.SaveChanges();
                        }

                    }
                }
                else
                {
                    var searchDataShipping = _graphiczoneDBContext.Shipping.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
                    if (searchDataShipping != null)
                    {
                        getname = searchDataShipping.ShippingFile;
                        searchDataShipping.OrPrintId = orderPrint.OrPrintId;
                        searchDataShipping.ShippingDate = shipping.ShippingDate;
                        searchDataShipping.ShippingFile = getname;
                        searchDataShipping.UserId = getUserid.UserId;
                        _graphiczoneDBContext.SaveChanges();
                    }
                }

                var x = "คุณได้ทำการบันทึกการส่งมอบ รายการที่ : " + orderPrint.OrPrintId + "เรียบร้อยแล้ว";
                return RedirectToAction("ListWorkAll");
            }

            return Json("ไม่สำเร็จ");
        }

        [HttpPost]
        public JsonResult deleteshipping(OrderPrint orderPrint, Shipping shipping)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;
                _graphiczoneDBContext.Remove(shipping);
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpPost]
        public JsonResult deleteworkstatus(OrderPrint orderPrint, Shipping shipping)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            var searchData2 = _graphiczoneDBContext.Shipping.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if(searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;
                if (searchData2 != null)
                {
                    _graphiczoneDBContext.Remove(searchData2);
                }
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpPost]
        public JsonResult deletepayment(OrderPrint orderPrint, Shipping shipping, ProofPayment proofPayment)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            var searchData2 = _graphiczoneDBContext.Shipping.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            var searchData3 = _graphiczoneDBContext.ProofPayment.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            if (searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;
                //if (searchData2 != null)
                //{
                //    _graphiczoneDBContext.Remove(searchData2);
                //}
                //if(searchData3 != null)
                //{
                //    _graphiczoneDBContext.Remove(searchData3);
                //}
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public IActionResult ListWorkStatus(string id)
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return View("../User/Login");
            }
            else
            {
                if(id != null)
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 2 && x.OrPrintId == id).ToList();
                    ViewBag.countData = searchData.Count();
                    return View(searchData);
                }
                else
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 2 && id == null).ToList();
                    ViewBag.countData = searchData.Count();
                    return View(searchData);
                }
            }
        }

        public IActionResult ListOverdue(string id)
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return View("../User/Login");
            }
            else
            {
                if (id != null)
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 0 && x.OrPrintId == id && x.OrPrintDue >= dt.AddDays(4)).ToList();
                    ViewBag.countData = searchData.Count();
                    return View(searchData);
                }
                else
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 0 && id == null && x.OrPrintDue >= dt.AddDays(4)).ToList();
                    ViewBag.countData = searchData.Count();
                    return View(searchData);
                }
            }
        }

        [HttpGet]
        public IActionResult Edit(string id)
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

        [HttpGet]
        public IActionResult OrderDetailforOverdue(string id)
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
                    ViewBag.OrderDue = seachData2.OrPrintDue;
                    var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                    ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                    ViewBag.TotalDate = seachData2.OrPrintDue - DateTime.Now;
                    var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == seachData.PrintId).FirstOrDefault();
                    ViewBag.PrintName = printname.PrintName;
                    ViewBag.PriceTotal = seachData2.OrPrintTotal;
                }
                return PartialView("_OrderDetailforOverdue", seachData);
            }
        }

        [HttpGet]
        public IActionResult OrderDetailforBin(string id)
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
                    ViewBag.OrderDue = seachData2.OrPrintDue;
                    var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                    ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                    ViewBag.TotalDate = seachData2.OrPrintDue - DateTime.Now;
                    var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == seachData.PrintId).FirstOrDefault();
                    ViewBag.PrintName = printname.PrintName;
                    ViewBag.PriceTotal = seachData2.OrPrintTotal;
                }
                return PartialView("_OrderDetailforBin", seachData);
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

        public ViewResult ListReport()
        {
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Report(int radvalue, DateTime datestart, DateTime dateend)
        {
            string dates = datestart.AddYears(543).ToString("dd MMM yyyy");
            string datee = dateend.AddYears(543).ToString("dd MMM yyyy");
            HttpContext.Session.SetString("dates", dates);
            HttpContext.Session.SetString("datee", datee);
            if (radvalue == 0)
            {
                return Json(0);
            }
            else if (radvalue == 1)
            {
                return Json(1);
            }
            else if (radvalue == 2)
            {
                return Json(2);
            }
            else if (radvalue == 3)
            {
                return Json(3);
            }
            else if (radvalue == 4)
            {
                return Json(4);
            }
            else if (radvalue == 5)
            {
                return Json(5);
            }
            else if (radvalue == 6)
            {
                return Json(6);
            }
            else if (radvalue == 7)
            {
                return Json(7);
            }
            else if (radvalue == 8)
            {
                return Json(8);
            }
            else if (radvalue == 9)
            {
                return Json(9);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Reportservice()
        {
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchReSer = _graphiczoneDBContext.TypePrint.ToList();
                if (searchReSer != null)
                {
                    List<Print> prints = _graphiczoneDBContext.Print.ToList();
                    ViewBag.listprint = prints;
                }
                HttpContext.Session.Remove("dates");
                HttpContext.Session.Remove("datee");
                return View("../ViewReport/ReportService", searchReSer);
            }
        }
        public IActionResult ReportAllincome()
        {
            DateTime Start = Convert.ToDateTime(HttpContext.Session.GetString("dates"));
            DateTime End = Convert.ToDateTime(HttpContext.Session.GetString("datee"));
            ViewBag.Start = Start;
            ViewBag.End = End;
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchReAllincome = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus <= 4 && x.OrPrintDate >= Start && x.OrPrintDate <= End).ToList();
                if (searchReAllincome != null)
                {
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
                    ViewBag.list = orderDetailPrints;
                    List<Print> prints = _graphiczoneDBContext.Print.ToList();
                    ViewBag.listprint = prints;
                    List<Customer> customers = _graphiczoneDBContext.Customer.ToList();
                    ViewBag.listcus = customers;
                }
                HttpContext.Session.Remove("dates");
                HttpContext.Session.Remove("datee");
                return View("../ViewReport/ReportAllincome", searchReAllincome);
            }
        }

        public IActionResult ReportPaid()
        {
            DateTime Start = Convert.ToDateTime(HttpContext.Session.GetString("dates"));
            DateTime End = Convert.ToDateTime(HttpContext.Session.GetString("datee"));
            ViewBag.Start = Start;
            ViewBag.End = End;
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchRePaid = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus >= 2 && x.OrPrintDate >= Start && x.OrPrintDate <= End).ToList();
                if (searchRePaid != null)
                {
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
                    ViewBag.list = orderDetailPrints;
                    List<Print> prints = _graphiczoneDBContext.Print.ToList();
                    ViewBag.listprint = prints;
                    List<Customer> customers = _graphiczoneDBContext.Customer.ToList();
                    ViewBag.listcus = customers;
                }
                HttpContext.Session.Remove("dates");
                HttpContext.Session.Remove("datee");
                return View("../ViewReport/ReportPaid", searchRePaid);
            }
        }

        public IActionResult ReportOverdue()
        {
            DateTime Start = Convert.ToDateTime(HttpContext.Session.GetString("dates"));
            DateTime End = Convert.ToDateTime(HttpContext.Session.GetString("datee"));
            ViewBag.Start = Start;
            ViewBag.End = End;
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchReOverdue = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus < 2 && x.OrPrintDate >= Start && x.OrPrintDate <= End).ToList();
                if (searchReOverdue != null)
                {
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
                    ViewBag.list = orderDetailPrints;
                    List<Print> prints = _graphiczoneDBContext.Print.ToList();
                    ViewBag.listprint = prints;
                    List<Customer> customers = _graphiczoneDBContext.Customer.ToList();
                    ViewBag.listcus = customers;
                }
                HttpContext.Session.Remove("dates");
                HttpContext.Session.Remove("datee");
                return View("../ViewReport/ReportOverdue", searchReOverdue);
            }
        }

        public IActionResult ReportWorking()
        {
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchReWorking = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 2).ToList();
                if (searchReWorking != null)
                {
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
                    ViewBag.list = orderDetailPrints;
                    List<Print> prints = _graphiczoneDBContext.Print.ToList();
                    ViewBag.listprint = prints;
                    List<Customer> customers = _graphiczoneDBContext.Customer.ToList();
                    ViewBag.listcus = customers;
                }
                HttpContext.Session.Remove("dates");
                HttpContext.Session.Remove("datee");
                return View("../ViewReport/ReportWorking", searchReWorking);
            }
        }

        public IActionResult ReportWorkdone()
        {
            DateTime Start = Convert.ToDateTime(HttpContext.Session.GetString("dates"));
            DateTime End = Convert.ToDateTime(HttpContext.Session.GetString("datee"));
            ViewBag.Start = Start;
            ViewBag.End = End;
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchReWorkdone = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 3 && x.OrPrintDate >= Start && x.OrPrintDate <= End).ToList();
                if (searchReWorkdone != null)
                {
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
                    ViewBag.list = orderDetailPrints;
                    List<Print> prints = _graphiczoneDBContext.Print.ToList();
                    ViewBag.listprint = prints;
                    List<Customer> customers = _graphiczoneDBContext.Customer.ToList();
                    ViewBag.listcus = customers;
                }
                HttpContext.Session.Remove("dates");
                HttpContext.Session.Remove("datee");
                return View("../ViewReport/ReportWorkdone", searchReWorkdone);
            }
        }

        public IActionResult ReportShipping()
        {
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchReShiping = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 3).ToList();
                if (searchReShiping != null)
                {
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
                    ViewBag.list = orderDetailPrints;
                    List<Print> prints = _graphiczoneDBContext.Print.ToList();
                    ViewBag.listprint = prints;
                    List<Customer> customers = _graphiczoneDBContext.Customer.ToList();
                    ViewBag.listcus = customers;
                }
                HttpContext.Session.Remove("dates");
                HttpContext.Session.Remove("datee");
                return View("../ViewReport/ReportShipping", searchReShiping);
            }
        }

        public IActionResult ReportShippingFinish()
        {
            DateTime Start = Convert.ToDateTime(HttpContext.Session.GetString("dates"));
            DateTime End = Convert.ToDateTime(HttpContext.Session.GetString("datee"));
            ViewBag.Start = Start;
            ViewBag.End = End;
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchShipping = _graphiczoneDBContext.Shipping.Where(x => x.ShippingDate >= Start && x.ShippingDate <= End).ToList();
                if(searchShipping != null)
                {
                    List<Shipping> shippings = _graphiczoneDBContext.Shipping.ToList();
                    ViewBag.shipping = shippings;
                    var searchReShipingFinish = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus >= 4).ToList();
                    if (searchReShipingFinish != null)
                    {
                        List<OrderPrint> orderPrints = _graphiczoneDBContext.OrderPrint.ToList();
                        ViewBag.listor = orderPrints;
                        List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
                        ViewBag.list = orderDetailPrints;
                        List<Print> prints = _graphiczoneDBContext.Print.ToList();
                        ViewBag.listprint = prints;
                        List<Customer> customers = _graphiczoneDBContext.Customer.ToList();
                        ViewBag.listcus = customers;
                    }
                }
                HttpContext.Session.Remove("dates");
                HttpContext.Session.Remove("datee");
                return View("../ViewReport/ReportShippingFinish", searchShipping);
            }
        }

        public IActionResult ReportUser()
        {
            if (HttpContext.Session.GetString("AdminUsername") == null)
            {
                return View("../Admin/Login");
            }
            else
            {
                getViewbagshopdetail();
                var searchReUser = _graphiczoneDBContext.User.ToList();
                return View("../ViewReport/ReportUser", searchReUser);
            }
        }

        public void getViewbagshopdetail()
        {
            var searchShop = _graphiczoneDBContext.Shop.Where(x => x.ShopId == "SG001").FirstOrDefault();
            if (searchShop != null)
            {
                ViewBag.getShoplogo = searchShop.ShopLogo;
                ViewBag.getShopname = searchShop.ShopName;
                ViewBag.getShopaddress = searchShop.ShopAddress;
                ViewBag.getShoptel = searchShop.ShopTel;
                ViewBag.getShopfax = searchShop.ShopFax;
                ViewBag.getShopemail = searchShop.ShopEmail;
                ViewBag.getShopline = searchShop.ShopLine;
            }
        }
    }
}
