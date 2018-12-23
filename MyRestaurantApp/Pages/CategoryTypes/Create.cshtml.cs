using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRestaurantApp.Data;
using MyRestaurantApp.Utility;

namespace MyRestaurantApp.Pages.CategoryTypes
{
    [Authorize(Policy =SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CategoryType CategoryType { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _db.CategoryType.Add(CategoryType);
            await _db.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}