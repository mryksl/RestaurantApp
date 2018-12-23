using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Data;

namespace MyRestaurantApp.Pages.MenuItems
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext context)
        {
            _db = context;
        }

        public IList<MenuItem> MenuItems { get; set; }

        public async Task OnGet()
        {
            MenuItems = await _db.MenuItem.Include(x=>x.CategoryType).Include(x=>x.FoodType).ToListAsync();
        }
    }
}