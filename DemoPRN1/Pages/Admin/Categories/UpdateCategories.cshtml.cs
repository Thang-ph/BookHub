using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.Admin.Categories
{
    public class UpdateCategoriesModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public UpdateCategoriesModel(PJPRN221Context context)
        {
            _context = context;

        }
        [BindProperty]
        public Category updateCategory { get; set; } 
        public List<Category> categories { get; set; }
        public IActionResult OnGet(int id)
        {
            updateCategory = _context.Categories.FirstOrDefault(c=>c.CategoryId==id);
            return Page();
        }

        public IActionResult OnPost() {
            if (!ModelState.IsValid)
            {              
                return Page();
            }
            _context.Categories.Update(updateCategory);
            _context.SaveChanges();
            return RedirectToPage("/Admin/Categories/ViewCategory");
        }
    }
}
