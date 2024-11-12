using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static DemoPRN1.Pages.Admin.dashboard.IndexModel;

namespace DemoPRN1.Pages.Admin.dashboard
{
    public class IndexModel : PageModel
    {
        private readonly PJPRN221Context _context;
        [BindProperty]
        public int transactionsCount { get; set; }
        [BindProperty]
        public int? booksSoldCount { get; set; }
        [BindProperty]
        public int? booksRentedCount { get; set; }
        [BindProperty]
        public int userCount { get; set; }
        [BindProperty]
        public decimal? TotalRevenue { get; set; }
        public List<MonthlyRevenue> MonthlyRevenues { get; set; }
        public IndexModel(PJPRN221Context context)
        {
            _context = context;
        }
        public class MonthlyRevenue
        {
            public int Month { get; set; }
            public int Year { get; set; }
            public decimal Revenue { get; set; }
        }
        public async Task<IActionResult> OnGet()
        {
            transactionsCount = await _context.Oders.CountAsync();
            booksSoldCount = await _context.Oderdetails
                                   .Where(od => od.Type == true)
                                   .SumAsync(od => od.Quantity);
            booksRentedCount = await _context.Oderdetails
                                   .Where(od => od.Type == false)
                                   .SumAsync(od => od.Quantity);
            userCount = await _context.Accounts.CountAsync();
            var purchaseRevenue = _context.Oderdetails
             .Where(od => od.Type == true) // Giao dịch mua
             .Join(_context.Books,
                   od => od.BookId,
                   b => b.BookId,
                   (od, b) => new { od.Quantity, b.Price })
             .Sum(x => x.Quantity * x.Price);

            // Tính doanh thu cho các đơn hàng thuê
            var rentalRevenue = _context.Oderdetails
                .Where(od => od.Type == false && od.EndDate.HasValue) // Giao dịch thuê
                .Join(_context.Books,
                      od => od.BookId,
                      b => b.BookId,
                      (od, b) => new { od.Quantity, b.RentalPrice, od.Startdate, od.EndDate })
                .Sum(x => (EF.Functions.DateDiffDay(x.Startdate, x.EndDate.Value)) * x.Quantity * x.RentalPrice);

            // Tổng doanh thu
            TotalRevenue = (purchaseRevenue + rentalRevenue)*10/100;

            return Page();
        }
    }

       
}