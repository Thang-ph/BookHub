using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DemoPRN1.Pages.BookStore
{
    public class DashboardModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public DashboardModel(PJPRN221Context context)
        {
            _context = context;
        }
        public List<CategoryRevenue> CategoryRevenues { get; set; }
        public List<BookStatusCount> BookStatusCounts { get; set; }
        public List<MonthlyRevenue> MonthlyRevenues { get; set; }

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var bookStore = _context.Bookstores.Include(a => a.Books).FirstOrDefault(a => a.AccountId == userId.Value);
            var monthlyRevenueQuery = from order in _context.Oders
                                      join orderDetail in _context.Oderdetails on order.OrderdetailsId equals orderDetail.OrderdetailsId
                                      join book in _context.Books on orderDetail.BookId equals book.BookId
                                      where order.CreateAt.HasValue && book.BookStoreId == bookStore.BookStoreId 
                                      group new { order, orderDetail, book } by new
                                      {
                                          Month = order.CreateAt.Value.Month,
                                          Year = order.CreateAt.Value.Year
                                      } into g
                                      select new MonthlyRevenue
                                      {
                                          Month = g.Key.Month,
                                          Year = g.Key.Year,
                                          Revenue = g.Sum(x => (x.orderDetail.Startdate.HasValue && x.orderDetail.EndDate.HasValue)
                                                  ? (x.book.RentalPrice ?? 0) * (x.orderDetail.Quantity ?? 0)
                                                  : (x.book.Price ?? 0) * (x.orderDetail.Quantity ?? 0))
                                      };


            MonthlyRevenues = monthlyRevenueQuery.ToList();

            var bookStatusQuery = from book in _context.Books
                                  where book.BookStoreId == bookStore.BookStoreId 
                                  group book by new
                                  {
                                      Status = book.Status.HasValue ? (book.Status.Value ? 1 : 0) : (int?)null
                                  } into t
                                  select new BookStatusCount
                                  {
                                      Status = t.Key.Status,
                                      Count = t.Count()
                                  };

            BookStatusCounts = bookStatusQuery.ToList();
            var categoryRevenueQuery = from order in _context.Oders
                                       join orderDetail in _context.Oderdetails on order.OrderdetailsId equals orderDetail.OrderdetailsId
                                       join book in _context.Books on orderDetail.BookId equals book.BookId
                                       join category in _context.Categories on book.CategoryId equals category.CategoryId
                                       where book.BookStoreId == bookStore.BookStoreId && order.CreateAt.HasValue
                                       group new { order, orderDetail, book, category } by category.CategoryName into g
                                       select new CategoryRevenue
                                       {
                                           CategoryName = g.Key,
                                           Revenue = g.Sum(x => (x.orderDetail.Startdate.HasValue && x.orderDetail.EndDate.HasValue)
                                               ? (x.book.RentalPrice ?? 0) * (x.orderDetail.Quantity ?? 0)
                                               : (x.book.Price ?? 0) * (x.orderDetail.Quantity ?? 0))
                                       };

            CategoryRevenues = categoryRevenueQuery.ToList();

            var totalOrdersQuery = from orderDetail in _context.Oderdetails
                                   join book in _context.Books on orderDetail.BookId equals book.BookId
                                   where book.BookStoreId == bookStore.BookStoreId
                                   select orderDetail;

            int totalOrders = totalOrdersQuery.Count();

            int totalBooksSold = totalOrdersQuery
                                 .Where(x => !x.Startdate.HasValue && !x.EndDate.HasValue)
                                 .Sum(x => x.Quantity ?? 0);

            int totalBooksRented = totalOrdersQuery
                                   .Where(x => x.Startdate.HasValue && x.EndDate.HasValue)
                                   .Sum(x => x.Quantity ?? 0);

            decimal totalRevenue = totalOrdersQuery
                                   .Sum(x => (x.Startdate.HasValue && x.EndDate.HasValue)
                                              ? (x.Book.RentalPrice ?? 0) * (x.Quantity ?? 0)
                                              : (x.Book.Price ?? 0) * (x.Quantity ?? 0));

            ViewData["TotalOrders"] = totalOrders;
            ViewData["TotalBooksSold"] = totalBooksSold;
            ViewData["TotalBooksRented"] = totalBooksRented;
            ViewData["TotalRevenue"] = totalRevenue;




            return Page();
        }
    }

    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
    }

    public class BookStatusCount
    {
        public int? Status { get; set; } 
        public int Count { get; set; }  
    }
    public class CategoryRevenue
    {
        public string CategoryName { get; set; } 
        public decimal Revenue { get; set; }    
    }
}
