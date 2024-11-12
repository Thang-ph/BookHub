using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.Admin.Categories
{
    public class CreateCategoriesModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public CreateCategoriesModel(PJPRN221Context context)
        {
            _context = context;

        }
        [BindProperty]
        public Category category { get; set; } =new Category();
        public List<Category> Categories { get; set; }
        public void OnGet()
        {
          

        }
        public IActionResult OnPost() {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Categories=_context.Categories.ToList();
            foreach (var cate in Categories) {
                if (cate.CategoryName.ToLower().Equals(category.CategoryName.ToLower())){
                    ModelState.AddModelError(string.Empty, "Thể loại đã tồn tại");
                    return Page();
                }
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToPage("/Admin/Categories/ViewCategory");
        }    
    }
}
