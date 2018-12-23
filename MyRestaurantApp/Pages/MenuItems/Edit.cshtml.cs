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
using MyRestaurantApp.ViewModel;

namespace MyRestaurantApp.Pages.MenuItems
{
    [Authorize(Policy = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _iHost;

        public EditModel(ApplicationDbContext context, IHostingEnvironment ihost)
        {
            _db = context;
            _iHost = ihost;
        }
        
        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }


        public IActionResult OnGet(int? id)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            MenuItemVM = new MenuItemViewModel
            {
                MenuItem = _db.MenuItem.Include(x => x.CategoryType).Include(x => x.FoodType).SingleOrDefault(x => x.Id == id),
                CategoryType = _db.CategoryType.ToList(),
                FoodType = _db.FoodType.ToList()
            };

            if (MenuItemVM.MenuItem ==null)
            {
                return NotFound();
            }

            return Page();


        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string webRootPath = _iHost.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            if (files[0] != null && files[0].Length>0)
            {

                var uploads = Path.Combine(webRootPath,"images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                if (System.IO.File.Exists(Path.Combine(uploads,MenuItemVM.MenuItem.Id+extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension));
                }
                using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }
   
            _db.Attach(MenuItemVM.MenuItem).State=EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}