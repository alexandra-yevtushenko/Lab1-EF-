using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PineappleShop.Controllers
{

    [Authorize]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly pineapple_shopModel.pineapple_shopModel _shopModel;
        public OrderController(ILogger<OrderController> logger)
        {
            _shopModel = new pineapple_shopModel.pineapple_shopModel();
            _logger = logger;
        }

        public IActionResult Index(int[] ids)
        {
            List<pineapple_shopModel.PineappleMenu> orderList = new List<pineapple_shopModel.PineappleMenu>();
            foreach(var id in ids)
            {
                orderList.Add(_shopModel.PineappleMenus.FirstOrDefault(p => p.Id == id));
            }
            return View("Index", orderList);
        }
    }
}
