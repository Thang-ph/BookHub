
using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DemoPRN1.Pages.Custommer
{
    public class BookDetailModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public Book currentBook;
        public List<Book> recomendBooks;
        public List<Bookrating> bookRatings;
        public int accountId;
        public Account accounts { get; set; }
        public BookDetailModel(PJPRN221Context context)
        {
            _context = context;
            bookRatings = new List<Bookrating>();
        }

        public async Task<IActionResult> OnGetAsync(int bookId)
        {
            // Kiểm tra và lấy `UserId` từ session trong phương thức này thay vì constructor
            var userId = HttpContext.Session.GetInt32("UserId");
            accounts = _context.Accounts.FirstOrDefault(x => x.AccountId == userId.Value);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login"); // Redirect nếu chưa đăng nhập
            }
            accountId = (int)userId;

            if (bookId != 0)
            {
                currentBook = _context.Books.Include(b => b.BookStore).FirstOrDefault(b => b.BookId == bookId);
            }

            // Lấy thông tin giỏ hàng từ Session
            var cart = HttpContext.Session.GetString("addCart");
            Dictionary<string, int> cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);
            TempData["CartCount"] = cartItems.Count;

            recomendBooks = GetRandomBooks(await _context.Books.ToListAsync(), 4);
            bookRatings = _context.Bookratings
                                  .Where(x => x.BookId == bookId).Include(x => x.Account)
                                  .ToList();
            return Page();
        }

        public List<Book> GetRandomBooks(List<Book> books, int count)
        {
            Random random = new Random();
            return books.OrderBy(b => random.Next()).Take(count).ToList();
        }

        public async Task<IActionResult> OnPostAddToCart(int bookId, string type)
        {
            var cart = HttpContext.Session.GetString("addCart");

            // Lấy thông tin giỏ hàng từ Session
            Dictionary<string, int> cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);

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

            return await OnGetAsync(bookId);
        }
        public async Task<IActionResult> OnPostAddComment(string comment, int bookId, int accountId)
        {
            Bookrating b = new Bookrating
            {
                Comment = comment,
                BookId = bookId,
                AccountId = accountId
            };

            _context.Bookratings.Add(b);
            await _context.SaveChangesAsync();

            return await OnGetAsync(bookId);
        }
    }
}