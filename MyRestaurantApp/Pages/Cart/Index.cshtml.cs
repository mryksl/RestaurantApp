using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Data;
using MyRestaurantApp.Utility;
using MyRestaurantApp.ViewModel;

namespace MyRestaurantApp.Pages.Cart
{
    public class IndexModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public void OnGet()
        {
            detailCart = new OrderDetailsCart
            {
                OrderHeader = new OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;

            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value);

            if (cart!=null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var item in detailCart.listCart)
            {
                item.MenuItem = _db.MenuItem.FirstOrDefault(x => x.Id == item.MenuItemId);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (item.MenuItem.Price * item.Count);
                if (item.MenuItem.Description.Length>1000)
                {
                    item.MenuItem.Description = item.MenuItem.Description.Substring(0, 99) + "...";
                }
                detailCart.OrderHeader.PickUpTime = DateTime.Now;
            }

        }

        public async Task<IActionResult> OnPostPlus(int cartId)
        {
            var cart =await _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefaultAsync();
            cart.Count += 1;
            _db.SaveChanges();


            return RedirectToPage("/Cart/Index");
        }

        public async Task<IActionResult> OnPostMinus(int cartId)
        {
            var cart = await _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefaultAsync();
            if (cart.Count==1)
            {
                _db.ShoppingCart.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32("CartCount", _db.ShoppingCart.Where(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count);

            }
            else
            {
                cart.Count -= 1;
                _db.SaveChanges();
            }
       
            _db.SaveChanges();

            return RedirectToPage("/Cart/Index");
        }

        public async Task<IActionResult> OnPost()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;

            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            detailCart.listCart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToList();

            OrderHeader orderHeader = detailCart.OrderHeader;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.UserId = claim.Value;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.Status = SD.StatusSubmitted;
       
            _db.OrderHeader.Add(orderHeader);
            _db.SaveChanges();


            foreach (var item in detailCart.listCart)
            {
                item.MenuItem = _db.MenuItem.FirstOrDefault(x => x.Id == item.MenuItemId);
                OrderDetail orderDetails = new OrderDetail
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = orderHeader.Id,
                    Name = item.MenuItem.Name,
                    Description = item.MenuItem.Description,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                _db.OrderDetail.Add(orderDetails);

            }
            //Since the user ordered his food. Then I need to remove shopping card for the current user and session to 0.
            _db.ShoppingCart.RemoveRange(detailCart.listCart);
            HttpContext.Session.SetInt32("CartCount", 0);
            _db.SaveChanges();
            return RedirectToPage("../Order/OrderConfirmation", new { id=orderHeader.Id });
        }
    }
}