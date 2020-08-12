using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Graphiczone.Models;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graphiczone.Controllers
{
    public class OrderPrintController : Controller
    {
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
                return RedirectToAction("");
            }
            else
            {
                var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id || id == null ).ToList();
                return View(searchData);
            }
        }
        [HttpGet]
        public IActionResult GetWorkDetail(string id)
        {
            if(id == "0")
            {
                return View();
            }
            else
            {
                var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
                if(searchData != null)
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
                    ViewBag.getDueForPicker = searchData.OrPrintDue.Value.AddYears(-543).ToString("yyyy-MM-dd");

                    var calOrPrintData = searchData.OrPrintDue - DateTime.Now;
                    ViewBag.OrPrintTotalDate = calOrPrintData.Value.ToString("dd");
                    var searchDataOrDetail = _graphiczoneDBContext.OrderDetailPrint.Where(ord => ord.OrPrintId == id).FirstOrDefault();
                    if(searchDataOrDetail != null)
                    {
                        List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.Where(ord => ord.OrPrintId == id).ToList();
                        ViewBag.OrPrintDetail = orderDetailPrints;
                        var printname = _graphiczoneDBContext.Print.Where(p => p.PrintId == searchDataOrDetail.PrintId).FirstOrDefault();
                        ViewBag.PrintName = printname.PrintName;
                        ViewBag.PriceTotal = searchData.OrPrintTotal;
                    }
                    var searchDataProofpayment = _graphiczoneDBContext.ProofPayment.Where(x => x.OrPrintId == id).FirstOrDefault();
                    if(searchDataProofpayment != null)
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
                    if(searchDataShipping != null)
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
                shipping.UserId = getUserid.UserId;

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
                if (searchData2 != null)
                {
                    _graphiczoneDBContext.Remove(searchData2);
                }
                if(searchData3 != null)
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

        public IActionResult ListWorkStatus(string id)
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return RedirectToAction("");
            }
            else
            {
                if(id != null)
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 1 && x.OrPrintId == id).ToList();
                    return View(searchData);
                }
                else
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus == 1 && id == null).ToList();
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
