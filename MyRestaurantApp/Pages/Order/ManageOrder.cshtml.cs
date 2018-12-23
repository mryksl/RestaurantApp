using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRestaurantApp.Data;
using MyRestaurantApp.Utility;
using MyRestaurantApp.ViewModel;

namespace MyRestaurantApp.Pages.Order
{
    public class ManageOrderModel : PageModel
    {
        private ApplicationDbContext _db;

        public ManageOrderModel(ApplicationDbContext db)
        {
            _db = db;
            OrderDetailViewModel = new List<OrderDetailViewModel>();
        }

        [BindProperty]
        public List<OrderDetailViewModel> OrderDetailViewModel { get; set; }

        public void OnGet()
        {
            List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(u => u.Status != SD.StatusCompleted && u.Status != SD.StatusReady && u.Status != SD.StatusCancelled).OrderByDescending(X => X.OrderDate).ToList();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailViewModel individual =new ViewModel.OrderDetailViewModel();
                individual.OrderDetail = _db.OrderDetail.Where(x => x.OrderId == item.Id).ToList();
                individual.OrderHeader = item;

                OrderDetailViewModel.Add(individual);
            }
        }

        public async Task<IActionResult> OnPostOrderPrepare(int id)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(id);
            orderHeader.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToPage("/Order/ManageOrder");
        }

        public async Task<IActionResult> OnPostOrderReady(int id)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(id);
            orderHeader.Status = SD.StatusReady;
            await _db.SaveChangesAsync();
            return RedirectToPage("/Order/ManageOrder");
        }

        public async Task<IActionResult> OnPostOrderCancel(int id)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(id);
            orderHeader.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();
            return RedirectToPage("/Order/ManageOrder");
        }

     

    }
}