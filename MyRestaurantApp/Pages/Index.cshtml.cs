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
using MyRestaurantApp.ViewModel;

namespace MyRestaurantApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public IndexViewModel IndexViewModel { get; set; }

        public async Task OnGet()
        {
            IndexViewModel = new IndexViewModel()
            {
                MenuItems = await _db.MenuItem.Include(x => x.CategoryType).Include(x => x.FoodType).ToListAsync(),
                CategoryTypes = await _db.CategoryType.OrderBy(x => x.DisplayOrder).ToListAsync(),
            };

            //This is how to get the Id of current user and set it.
          
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (claim!=null)
            {
       
                    var count = _db.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value).ToList().Count();

                    HttpContext.Session.SetInt32("CartCount", count);
             
            }
     
        

        }
    }
}
