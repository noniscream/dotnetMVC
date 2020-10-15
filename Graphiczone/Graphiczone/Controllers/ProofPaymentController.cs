using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Graphiczone.Models.SQLServer;

namespace Graphiczone.Controllers
{
    public class ProofPaymentController : Controller
    {
        DateTime dt = DateTime.Now;

        private readonly GraphiczoneDBContext _graphiczoneDBContext;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProofPaymentController(GraphiczoneDBContext graphiczoneDBContext, IWebHostEnvironment hostEnvironment)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
            this._webHostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CusUsername") == null)
            {
                return RedirectToAction("");
            }
            else
            {
                return View();
            }

        }

        public IActionResult ListProofPayment(string id, int pageNumber = 1, int pageSize = 10)
        {
            if (HttpContext.Session.GetString("CusUsername") == null)
            {
                return View("../Customer/Login");
            }
            else
            {
                int ExcludeRecords = (pageSize * pageNumber) - pageSize;
                var sessionid = HttpContext.Session.GetString("CusUsername");
                var getCusId = _graphiczoneDBContext.Customer.Where(x => x.CusUsername == sessionid).FirstOrDefault();
                var cusId = getCusId.CusId;
                if (id != null)
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus <= 1 && x.OrPrintId == id && x.CusId == cusId && x.OrPrintDue != null && x.OrPrintDue >= dt.AddDays(-2)).OrderByDescending(x => x.OrPrintId)//.ToList()
                        .Skip(ExcludeRecords)
                        .Take(pageSize);
                    var searchPage = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus <= 1 && x.OrPrintId == id && x.CusId == cusId && x.OrPrintDue != null && x.OrPrintDue >= dt.AddDays(-2)).OrderByDescending(x => x.OrPrintId).Count();
                    ViewBag.totalpage = searchPage;
                    ViewBag.pagenumber = pageNumber;
                    ViewBag.pagesize = pageSize;
                    ViewBag.countData = searchData.Count();
                    var searchPrf = _graphiczoneDBContext.ProofPayment.ToList();
                    if (searchPrf != null)
                    {
                        List<ProofPayment> prf = _graphiczoneDBContext.ProofPayment.ToList();
                        ViewBag.prfpay = prf;
                    }
                    return View(searchData);
                }
                else
                {
                    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus <= 1 && id == null && x.CusId == cusId && x.OrPrintDue != null && x.OrPrintDue >= dt.AddDays(-2)).OrderByDescending(x => x.OrPrintId)//.ToList()
                        .Skip(ExcludeRecords)
                        .Take(pageSize);
                    var searchPage = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintStatus <= 1 && id == null && x.CusId == cusId && x.OrPrintDue != null && x.OrPrintDue >= dt.AddDays(-2)).OrderByDescending(x => x.OrPrintId).Count();
                    ViewBag.totalpage = searchPage;
                    ViewBag.pagenumber = pageNumber;
                    ViewBag.pagesize = pageSize;
                    ViewBag.countData = searchData.Count();
                    var searchPrf = _graphiczoneDBContext.ProofPayment.ToList();
                    if (searchPrf != null)
                    {
                        List<ProofPayment> prf = _graphiczoneDBContext.ProofPayment.ToList();
                        ViewBag.prfpay = prf;
                    }
                    return View(searchData);
                }
            }
        }
        public IActionResult EditProofPayment(string id = "0")
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
                    var searchData3 = _graphiczoneDBContext.ProofPayment.Where(x => x.OrPrintId == id).FirstOrDefault();
                    if(searchData3 != null)
                    {
                        ViewBag.PrfPayFile = searchData3.PrfPayFile;
                        ViewBag.PrfPayStatus = searchData3.PrfPayStatus;

                    }
                    List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == id).ToList();
                    ViewBag.OrderList = orderDetailPrints;
                    ViewBag.OrderPrintId = seachData.OrPrintId;
                    ViewBag.OrderDate = seachData2.OrPrintDate;
                    ViewBag.OrderDue = seachData2.OrPrintDue;
                    ViewBag.GetDateForPicker = seachData2.OrPrintDate.Value.ToString("yyyy-MM-dd");
                    ViewBag.getDueForPicker = seachData2.OrPrintDue.Value.ToString("yyyy-MM-dd");
                    var searchDataOrder = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == id).FirstOrDefault();
                    var cusname = _graphiczoneDBContext.Customer.Where(c => c.CusId == seachData2.CusId).FirstOrDefault();
                    ViewBag.CusFullname = cusname.CusFirstname + " " + cusname.CusLastname;
                    ViewBag.TotalDate = seachData2.OrPrintDue - DateTime.Now;
                    var printname = _graphiczoneDBContext.Print.ToList();
                    List<Print> prints = printname.ToList();
                    ViewBag.PrintName = prints;
                    ViewBag.PriceTotal = seachData2.OrPrintTotal;
                }
                return PartialView("_EditProofPayment", seachData);
            }

        }

        public IFormFile Uploadfile { get; set; }

        [HttpPost]
        public async Task<IActionResult> uploadAsync(OrderPrint orderPrint, ProofPayment proofPayment)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();

            if (searchData != null)
            {
                searchData.OrPrintStatus = orderPrint.OrPrintStatus;

                proofPayment.OrPrintId = orderPrint.OrPrintId;
                proofPayment.PrfPayDate = proofPayment.PrfPayDate;

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(Uploadfile.FileName);
                string extension = Path.GetExtension(Uploadfile.FileName);

                if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                {
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    proofPayment.PrfPayFile = "images/" + fileName;
                    fileName = Path.Combine(wwwRootPath, "images", fileName);


                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await Uploadfile.CopyToAsync(fileStream);
                    }
                }

                _graphiczoneDBContext.ProofPayment.Add(proofPayment);
                _graphiczoneDBContext.SaveChanges();
                var x = "คุณได้ทำการแจ้งหลักฐานการโอน รายการที่ : " + orderPrint.OrPrintId + "เรียบร้อยแล้ว";
                return RedirectToAction("ListProofPayment");
            }

            return Json("ไม่สำเร็จ");
        }

        [HttpPost]
        public async Task<IActionResult> edituploadAsync(OrderPrint orderPrint, ProofPayment proofPayment)
        {
            var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();

            if (searchData != null)
            {
                var searchData2 = _graphiczoneDBContext.ProofPayment.Where(x => x.OrPrintId == proofPayment.OrPrintId).FirstOrDefault();
                if(searchData2 != null)
                {
                    _graphiczoneDBContext.Remove(searchData2);
                }

                searchData.OrPrintStatus = orderPrint.OrPrintStatus;

                proofPayment.OrPrintId = orderPrint.OrPrintId;
                proofPayment.PrfPayDate = proofPayment.PrfPayDate;

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(Uploadfile.FileName);
                string extension = Path.GetExtension(Uploadfile.FileName);

                if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                {
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    proofPayment.PrfPayFile = "images/" + fileName;
                    fileName = Path.Combine(wwwRootPath, "images", fileName);


                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await Uploadfile.CopyToAsync(fileStream);
                    }
                }
                _graphiczoneDBContext.ProofPayment.Add(proofPayment);
                _graphiczoneDBContext.SaveChanges();
                var x = "คุณได้ทำการแจ้งหลักฐานการโอน รายการที่ : " + orderPrint.OrPrintId + "เรียบร้อยแล้ว";
                return RedirectToAction("ListProofPayment");
            }

            return Json("ไม่สำเร็จ");
        }

    }
}
