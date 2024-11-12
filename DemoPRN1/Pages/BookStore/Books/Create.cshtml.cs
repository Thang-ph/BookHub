using DemoPRN1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly PJPRN221Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CreateModel(PJPRN221Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Book newBook { get; set; } = new Book();
        [BindProperty]
        public List<Category> Categories { get; set; }
        [BindProperty]
        public IFormFile ImageFile { get; set; }
        public async Task<IActionResult> OnGet()
        {
            Categories = _context.Categories.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories.ToListAsync();
                return Page();
            }
            if (ImageFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

                string fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;

                string filePath = Path.Combine(uploadFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                newBook.Image = "~/Images/" + fileName;
            }
            newBook.Status = false;
            Book b = _context.Books.FirstOrDefault(b => b.Isbn.Equals(newBook.Isbn));
          
            newBook.RentalQuantity = 0;
            newBook.CreateAt = DateTime.Now;
            newBook.UpdateAt = DateTime.Now;
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                // Truy vấn Account bao gồm Bookstores
                var account = _context.Accounts
                                      .Include(a => a.Bookstores)
                                      .FirstOrDefault(a => a.AccountId == userId.Value);

                // Kiểm tra nếu account có Bookstore liên kết
                if (account != null && account.Bookstores.Any())
                {
                    // Lấy Bookstore đầu tiên
                    newBook.BookStoreId = account.Bookstores.First().BookStoreId;
                }
            }
            _context.Books.Add(newBook);
            _context.SaveChanges();

            return RedirectToPage("/BookStore/Books/Index");
        }
    }
}
