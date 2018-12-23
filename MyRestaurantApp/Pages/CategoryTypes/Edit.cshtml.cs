﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Data;

namespace MyRestaurantApp.Pages.CategoryTypes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext context)
        {
            _db = context;
        }
        
        [BindProperty]
        public CategoryType CategoryType { get; set; } 

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            CategoryType = await _db.CategoryType.SingleOrDefaultAsync(c=>c.Id==id);

            if (CategoryType ==null)
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

            _db.Attach(CategoryType).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}