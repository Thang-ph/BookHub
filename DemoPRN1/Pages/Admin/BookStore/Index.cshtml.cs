using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.Admin.BookStore
{
    public class IndexModel : PageModel
    {
        private readonly PJPRN221Context _context = new PJPRN221Context();

        public List<Bookstore> bookstores { get; set; }
        public void OnGet()
        {
            bookstores = _context.Bookstores.Include(b => b.Account).ToList();
        }
        public IActionResult OnPost(int id)
        {
            var bookstore = _context.Bookstores.FirstOrDefault(b => b.BookStoreId == id);      
            if (bookstore == null)
            {
                return NotFound();
            }        
            bookstore.Status = false;            
            _context.Update(bookstore);
            _context.SaveChanges();
            return RedirectToPage("/Admin/BookStore/Index");
        }
    }
    
}
