using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.Admin.BookStore
{
    public class BookStoreModel : PageModel
    {
        private readonly PJPRN221Context _context = new PJPRN221Context();

        public List<Bookstore> bookstores { get; set; }

        public void OnGet()
        {
            bookstores = _context.Bookstores.Include(b => b.Account).Where(b => b.Status == false).ToList();
        }
        public IActionResult OnPost(int id)
        {
            var bookstore = _context.Bookstores.FirstOrDefault(b => b.BookStoreId == id);
            var Account = _context.Accounts.FirstOrDefault(b => b.AccountId == bookstore.AccountId);
            if (Account == null) {
                return NotFound();
            }
            if (bookstore == null)
            {
                return NotFound();
            }
            Account.RoleId = 3;
            bookstore.Status = true;
            _context.Update(Account);
            _context.SaveChanges();
            _context.Update(bookstore);
            _context.SaveChanges();
            return RedirectToPage("/Admin/BookStore/BookStore");
        }
    }
}
