using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Data;
using MyRestaurantApp.Utility;

namespace MyRestaurantApp.Pages.FoodTypes
{
    [Authorize(Policy = SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext context)
        {
            _db = context;
        }

        [BindProperty]
        public FoodType FoodType { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FoodType = await _db.FoodType.SingleOrDefaultAsync(x => x.Id == id);

            if (FoodType == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}