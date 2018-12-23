using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Data;
using MyRestaurantApp.Utility;

namespace MyRestaurantApp.Pages
{

    public class DetailsModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ShoppingCart CartObj { get; set; }

        public async Task OnGet(int id)
        {
            var menuItemInDb = await _db.MenuItem.Include(x => x.CategoryType).Include(x => x.FoodType).Where(x => x.Id == id).FirstOrDefaultAsync();

            CartObj = new ShoppingCart
            {
                MenuItemId = menuItemInDb.Id,
                MenuItem = menuItemInDb
            };
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (ModelState.IsValid)
            {
                //This is how I get the Id of logged in user.
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;

                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                

                //----------------------------------------//

                CartObj.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb =  _db.ShoppingCart.Where(x => x.ApplicationUserId == CartObj.ApplicationUserId && x.MenuItemId == CartObj.MenuItemId).FirstOrDefault();

                if (cartFromDb == null)
                {

                    _db.ShoppingCart.Add(CartObj);

                }
                else
                {
                    cartFromDb.Count += CartObj.Count;
                }

                _db.SaveChanges();

                //Add session and increment count.
                var count = _db.ShoppingCart.Where(x => x.ApplicationUserId == CartObj.ApplicationUserId).ToList().Count();

                HttpContext.Session.SetInt32("CartCount", count);
                StatusMessage = "Item added to cart";
                return RedirectToPage("Index");
            }
            else
            {

                var menuItemInDb = await _db.MenuItem.Include(x => x.CategoryType).Include(x => x.FoodType).Where(x => x.Id == id).FirstOrDefaultAsync();

                CartObj = new ShoppingCart
                {
                    MenuItemId = menuItemInDb.Id,
                    MenuItem = menuItemInDb
                };

                return Page();
            }

            
           
        }
    }
}