using DemoPRN1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.Books
{
    public class UpdateModel : PageModel
    {
        private readonly PJPRN221Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UpdateModel(PJPRN221Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Book updateBook { get; set; }
        [BindProperty]
        public List<Category> Categories { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }
        public async Task<IActionResult> OnGet(int id)
        {
            Categories = _context.Categories.ToList();

            updateBook = _context.Books.Include(b => b.Category).FirstOrDefault(b => b.BookId == id);

            if (updateBook == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ImageFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

                string fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;

                string filePath = Path.Combine(uploadFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                updateBook.Image = "~/Images/" + fileName;
            }

            updateBook.UpdateAt = DateTime.Now;
            _context.Books.Update(updateBook);
            _context.SaveChanges();
            return RedirectToPage("/BookStore/Books/Index");
        }
    }
}
