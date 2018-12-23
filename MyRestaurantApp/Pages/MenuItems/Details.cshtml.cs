using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Data;
using MyRestaurantApp.Utility;

namespace MyRestaurantApp.Pages.MenuItems
{
    [Authorize(Policy = SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _iHost;

        public DetailsModel(ApplicationDbContext context, IHostingEnvironment ihost)
        {
            _db = context;
            _iHost = ihost;
        }

        [BindProperty]
        public MenuItem MenuItem { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            MenuItem = await _db.MenuItem.Include(x => x.CategoryType).Include(x => x.FoodType).SingleOrDefaultAsync(x=>x.Id==id);

            if (MenuItem==null)
            {
                return NotFound();
            }

            return Page();

        }
    }
}