using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DemoPRN1.Pages.Custommer
{
    public class RegisterModel : PageModel
    {
        private readonly PJPRN221Context _context;
        public Account accounts {get; set;}
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RegisterModel(PJPRN221Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public IFormFile ImageFile { get; set; }
        public Bookstore newBooktore { get; set; } = new Bookstore();
        [BindProperty]
        public Models.Bookstore  Input { get; set; }

      
        public void OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            accounts = _context.Accounts.FirstOrDefault(x => x.AccountId == userId.Value);
        }
        
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
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

                newBooktore.IdentityImg = "~/Images/" + fileName;
            }
            int? userId = HttpContext.Session.GetInt32("UserId");
             
            Bookstore bookstore = _context.Bookstores.Include(x=>x.Account).FirstOrDefault(b=>b.AccountId == userId.Value);
            if (bookstore != null) {
                ModelState.AddModelError(string.Empty, "Tài khoản này đã được đăng kí.Vui lòng chờ xét duyệt");
                return Page();
            }

            newBooktore.StoreName = Input.StoreName;
            newBooktore.Address = Input.Address;
            newBooktore.BankNumber = Input.BankNumber;
            newBooktore.BankName = Input.BankName;
            newBooktore.Status = false;
            newBooktore.CreateAt = DateTime.Now;
            newBooktore.UpdateAt = DateTime.Now;
            newBooktore.AccountId = userId.Value;
            _context.Bookstores.Add(newBooktore);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Đăng ký thành công! Haỹ đợi để được xét duyệt";
            return Page();
        }

    }
}
