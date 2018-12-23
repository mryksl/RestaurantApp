using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRestaurantApp.Data;
using MyRestaurantApp.ViewModel;

namespace MyRestaurantApp.Pages.Order
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public OrderConfirmationModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public OrderDetailViewModel OrderDetailViewModel { get; set; }

        public void OnGet(int id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailViewModel = new OrderDetailViewModel
            {
                OrderHeader = _db.OrderHeader.Where(z => z.Id == id && z.UserId==claim.Value).FirstOrDefault(),
                OrderDetail = _db.OrderDetail.Where(x => x.OrderId == id).ToList()
            };
        }
    }
}