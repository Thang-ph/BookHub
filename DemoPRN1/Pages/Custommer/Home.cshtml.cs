using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoPRN1.Pages.Custommer
{
    public class HomeModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public HomeModel(PJPRN221Context context)
        {
            _context = context;
        }

        public Account accounts { get; set; }
        public List<Book> Allbook { get; set; } = new List<Book>();
        public List<Book> Limitbook { get; set; } = new List<Book>();
        public List<Book> BestSellBooks { get; set; } = new List<Book>();
        public List<Book> TopRatedBooks { get; set; } = new List<Book>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public async Task<IActionResult> OnGetAsync(int? categoryId, string searchString, int pageNumber = 1)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            { 
                accounts = _context.Accounts.FirstOrDefault(x => x.AccountId == userId.Value);
            }
            await GetTopSellingBooks();
            await GetTopRatedBooks();
            await GetCategoryBook();
            await GetAllBooks();
            await GetLimitBooks();

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCart(int bookId, string type)
        {
			int? userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null)
			{
				return RedirectToPage("/Accounts/Login");
			}
			var cart = HttpContext.Session.GetString("addCart");
            var cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);

            string cartTypeAndName = bookId.ToString() + "_" + type;
            if (cartItems.ContainsKey(cartTypeAndName))
            {
                cartItems[cartTypeAndName]++;
            }
            else
            {
                cartItems[cartTypeAndName] = 1;
            }

            HttpContext.Session.SetString("addCart", JsonConvert.SerializeObject(cartItems));
            TempData["CartCount"] = cartItems.Count;

            return await OnGetAsync(null,"",1);
        }

        private async Task GetAllBooks()
        {
            Allbook = new List<Book>();

            var bestSellingBooks = await (from b in _context.Books
                                          join od in _context.Oderdetails on b.BookId equals od.BookId into odGroup
                                          from od in odGroup.DefaultIfEmpty()
                                          group od by new { b.BookId, b.BookTitle } into g
                                          select new
                                          {
                                              BookId = g.Key.BookId,
                                              TotalSold = g.Sum(od => od != null ? od.Quantity : 0)
                                          })
                                          .OrderByDescending(g => g.TotalSold)
                                          .ToListAsync();

            foreach (var sellingBook in bestSellingBooks)
            {
                var book = await _context.Books.AsNoTracking().Where(x => x.Status == true).FirstOrDefaultAsync(b => b.BookId == sellingBook.BookId);
                if (book != null)
                {
                    Allbook.Add(book);
                }
            }
        }

        private async Task GetLimitBooks()
        {
            Limitbook = new List<Book>();

            var bestSellingBooks = await (from b in _context.Books
                                          join od in _context.Oderdetails on b.BookId equals od.BookId into odGroup
                                          from od in odGroup.DefaultIfEmpty()
                                          group od by new { b.BookId, b.BookTitle } into g
                                          select new
                                          {
                                              BookId = g.Key.BookId,
                                              TotalSold = g.Sum(od => od != null ? od.Quantity : 0)
                                          })
                                          .OrderByDescending(g => g.TotalSold)
                                          .Take(8)
                                          .ToListAsync();

            foreach (var sellingBook in bestSellingBooks)
            {
                var book = await _context.Books.AsNoTracking().Where(x => x.Status == true).FirstOrDefaultAsync(b => b.BookId == sellingBook.BookId);
                if (book != null)
                {
                    Limitbook.Add(book);
                }
            }
        }

        private async Task GetTopRatedBooks()
        {
            var topRatedBooks = await _context.Books
                .AsNoTracking()
                .Where(x => x.Status == true)
                .Select(b => new
                {
                    Book = b,
                    AverageRating = b.Bookratings.Average(br => br.RatingId)
                })
                .Where(b => b.AverageRating > 0)
                .OrderByDescending(b => b.AverageRating)
                .Take(4)
                .ToListAsync();

            TopRatedBooks = topRatedBooks.Select(b =>
            {
                b.Book.Price = Math.Round(b.Book.Price ?? 0, 2);
                return b.Book;
            }).ToList();
        }

        private async Task GetTopSellingBooks()
        {
            BestSellBooks = new List<Book>();

            var bestSellingBooks = await (from b in _context.Books
                                          join od in _context.Oderdetails on b.BookId equals od.BookId
                                          group od by new { b.BookId, b.BookTitle } into g
                                          select new
                                          {
                                              BookId = g.Key.BookId,
                                              TotalSold = g.Sum(od => od.Quantity)
                                          })
                                          .OrderByDescending(g => g.TotalSold)
                                          .Take(3)
                                          .ToListAsync();

            foreach (var sellingBook in bestSellingBooks)
            {
                var book = await _context.Books.AsNoTracking().Where(x => x.Status==true).FirstOrDefaultAsync(b => b.BookId == sellingBook.BookId);
                if (book != null)
                {
                    BestSellBooks.Add(book);
                }
            }
        }

        private async Task GetCategoryBook()
        {
            Categories = await _context.Categories.AsNoTracking().ToListAsync();
        }
    }
}
