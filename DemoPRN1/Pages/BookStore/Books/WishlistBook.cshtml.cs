using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.BookStore.Books
{
    public class WishlistBookModel : PageModel
    {
        private readonly PJPRN221Context _context = new PJPRN221Context();

        public List<Book> Books { get; set; }

        public void OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var bookStore = _context.Bookstores.Include(a => a.Books).FirstOrDefault(a => a.AccountId == userId.Value);
            Books = _context.Books.Include(b => b.Category).Where(b => b.BookStoreId == bookStore.BookStoreId && b.Status == false).ToList();
        }

    }
}
