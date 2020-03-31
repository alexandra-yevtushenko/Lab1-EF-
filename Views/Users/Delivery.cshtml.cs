using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pineapple_shopModel;

namespace PineappleShop
{
    public class DeliveryModel : PageModel
    {
        private readonly pineapple_shopModel.pineapple_shopModel _context;

        public DeliveryModel(pineapple_shopModel.pineapple_shopModel context)
        {
            _context = context;
        }

        public IList<DeliveryInfo> DeliveryInfo { get;set; }

        public async Task OnGetAsync()
        {
            DeliveryInfo = await _context.DeliveryInfos
                .Include(d => d.Staff)
                .Include(d => d.User).ToListAsync();
        }
    }
}
