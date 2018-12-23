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
    public class OrderHistoryModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        public OrderHistoryModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<OrderDetailViewModel> OrderDetailViewModel { get; set; }

        //if id=0 display only 5 past orders else display all the orders.
        public void OnGet(int id=0)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailViewModel = new List<OrderDetailViewModel>();

            List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(u => u.UserId == claim.Value).OrderByDescending(x => x.OrderDate).ToList();

            if (id==0)
            {
                OrderHeaderList = OrderHeaderList.Take(5).ToList();
            }

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailViewModel individual = new ViewModel.OrderDetailViewModel();
                individual.OrderHeader = item;
                individual.OrderDetail = _db.OrderDetail.Where(o => o.Id == item.Id).ToList();

                OrderDetailViewModel.Add(individual);
            }
        }
    }
}