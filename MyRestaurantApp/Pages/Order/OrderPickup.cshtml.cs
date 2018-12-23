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
    public class OrderPickupModel : PageModel
    {
        private ApplicationDbContext _db;

        public OrderPickupModel(ApplicationDbContext db)
        {
            _db = db;
            OrderDetailViewModel = new List<OrderDetailViewModel>();
        }

        [BindProperty]
        public List<OrderDetailViewModel> OrderDetailViewModel { get; set; }

        public void OnGet(string option=null, string search=null)
        {
            if (search!= null)
            {
                var user = new ApplicationUser();
                List<OrderHeader> OrderHeaderList = new List<OrderHeader>();

                if (option.Equals("order"))
                {
                    OrderHeaderList = _db.OrderHeader.Where(o => o.Id.Equals(Convert.ToInt32(search))).ToList();
                }
                else if (option.Equals("email"))
                {
                    user = _db.Users.Where(u => u.Email.ToLower().Contains(search.ToLower())).FirstOrDefault();
                }
                else if (option.Equals("phone"))
                {
                    user = _db.Users.Where(u => u.PhoneNumber.ToLower().Contains(search.ToLower())).FirstOrDefault();
                }
                else if (option.Equals("name"))
                {
                    user = _db.Users.Where(u => u.FirstName.ToLower().Contains(search.ToLower()) || u.LastName.ToLower().Contains(search.ToLower())).FirstOrDefault();
                }

                if (user != null || OrderHeaderList.Count>0)
                {
                    if (OrderHeaderList.Count.Equals(0))
                    {
                        OrderHeaderList = _db.OrderHeader.Where(o => o.UserId.Equals(user.Id)).OrderByDescending(x => x.PickUpTime).ToList();
                    }

                    foreach (OrderHeader item in OrderHeaderList)
                    {
                        OrderDetailViewModel individual = new ViewModel.OrderDetailViewModel();
                        individual.OrderDetail = _db.OrderDetail.Where(x => x.OrderId == item.Id).ToList();
                        individual.OrderHeader = item;

                        OrderDetailViewModel.Add(individual);
                    }
                }
            }
            else
            {
                List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(u => u.Status.Equals(SD.StatusReady)).OrderByDescending(X => X.OrderDate).ToList();

                foreach (OrderHeader item in OrderHeaderList)
                {
                    OrderDetailViewModel individual = new ViewModel.OrderDetailViewModel();
                    individual.OrderDetail = _db.OrderDetail.Where(x => x.OrderId == item.Id).ToList();
                    individual.OrderHeader = item;

                    OrderDetailViewModel.Add(individual);
                }
            }
          
        }
    }
}