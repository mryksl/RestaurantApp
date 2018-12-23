using System;
using System.Collections.Generic;
using System.IO;
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
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostEnv;

        public DeleteModel(ApplicationDbContext context,IHostingEnvironment Ihost)
        {
            _db = context;
            _hostEnv = Ihost;
        }

        [BindProperty]
        public MenuItem MenuItem { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            MenuItem =  _db.MenuItem.Include(x => x.CategoryType).Include(x => x.FoodType).SingleOrDefault(x => x.Id == id);

            if (MenuItem==null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            string webRootPath = _hostEnv.WebRootPath;

            MenuItem = await _db.MenuItem.FindAsync(id);

            if (MenuItem !=null)
            {
                var uploads = Path.Combine(webRootPath, "images");

                var extension =  MenuItem.Image.Substring(MenuItem.Image.LastIndexOf("."), MenuItem.Image.Length - MenuItem.Image.LastIndexOf("."));


                var ImagePath = Path.Combine(uploads, MenuItem.Id + extension);

                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                  
                }
                _db.MenuItem.Remove(MenuItem);

                await _db.SaveChangesAsync();

            }
            return RedirectToPage("./Index");

        }
    }
}