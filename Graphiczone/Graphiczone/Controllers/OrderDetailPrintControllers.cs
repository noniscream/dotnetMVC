using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graphiczone.Models.SQLServer;
using Microsoft.AspNetCore.Mvc;

namespace Graphiczone.Controllers
{
    public class OrderDetailPrintControllers : Controller
    {
        private readonly GraphiczoneDBContext _graphiczoneDBContext;
        public OrderDetailPrintControllers(GraphiczoneDBContext graphiczoneDBContext)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
