using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRestaurantApp.Data;
using MyRestaurantApp.Utility;
using MyRestaurantApp.ViewModel;

namespace MyRestaurantApp.Pages.Order
{
    public class OrderPickupDetailsModel : PageModel
    {
        private ApplicationDbContext _db;

        public OrderPickupDetailsModel(ApplicationDbContext db)
        {
            _db = db;
            OrderDetailViewModel = new OrderDetailViewModel();
        }

        [BindProperty]
        public OrderDetailViewModel OrderDetailViewModel { get; set; }

        public void OnGet(int orderId)
        {
            OrderDetailViewModel.OrderHeader = _db.OrderHeader.Where(x => x.Id.Equals(orderId)).FirstOrDefault();
            OrderDetailViewModel.OrderHeader.ApplicationUser = _db.Users.Where(u => u.Id.Equals(OrderDetailViewModel.OrderHeader.UserId)).FirstOrDefault();
            OrderDetailViewModel.OrderDetail = _db.OrderDetail.Where(o => o.OrderId.Equals(OrderDetailViewModel.OrderHeader.Id)).ToList();

        }

        public IActionResult OnPost(int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusCompleted;
            _db.SaveChanges();

            return RedirectToPage("/Order/ManageOrder");
        }
    }
}