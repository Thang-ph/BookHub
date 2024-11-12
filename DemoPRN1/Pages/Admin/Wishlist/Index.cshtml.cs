using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.Admin.Wishlist
{
    public class IndexModel : PageModel
    {
        private readonly PJPRN221Context _context = new PJPRN221Context();

        public List<Book> Books { get; set; }

        public void OnGet()
        {
            Books = _context.Books.Include(b => b.Category).Where(b => b.Status==false).ToList();
        }
        public IActionResult OnPost(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }
            book.Status = true;
            _context.Update(book);
            _context.SaveChanges(); 

            return RedirectToPage("/Admin/Wishlist/Index");
        }
        public async Task<IActionResult> OnPostMarkAsSoldAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book != null)
            {
                book.Status = null;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
