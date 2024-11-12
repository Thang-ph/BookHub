using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoPRN1.Pages.Admin.Categories
{
    public class ViewCategoryModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public ViewCategoryModel(PJPRN221Context context)
        {
            _context = context;

        }
        public List<Category> categories { get; set; }
        public IActionResult OnGet()
        {
            categories = _context.Categories.ToList();
            return Page();
        }
       public IActionResult OnpostDelete(int id)
        {
            var cate = _context.Categories.FirstOrDefault(b => b.CategoryId == id);
            if (cate == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(cate);
            _context.SaveChanges(); 

            return RedirectToPage("/Admin/Categories/ViewCategory");
        }  
    }
}
