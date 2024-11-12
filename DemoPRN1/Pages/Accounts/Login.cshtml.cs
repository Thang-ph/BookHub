using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
namespace DemoPRN1.Pages.Accounts
{
    public class LoginModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public LoginModel(PJPRN221Context context)
        {
            _context = context;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public bool LoginFailed { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Email không được bỏ trống")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; }


            [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
                var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == Input.Email && u.Password == Input.Password);

                if (user == null|| user.Status==false)
                {
                    LoginFailed = true;
                    return Page();
                }


            if (User != null) { }
            //user la admin
            if (user.RoleId==1&& user.Status==true)
            {
                HttpContext.Session.SetInt32("UserId", user.AccountId);
                return RedirectToPage("/Admin/Categories/ViewCategory");
			}
            //user la khach hang
            if (user.RoleId == 2 && user.Status == true)
            {

                HttpContext.Session.SetInt32("UserId", user.AccountId);
                return RedirectToPage("/Custommer/Home");
            }
            //user la Bookstore
            if (user.RoleId == 3 && user.Status == true) {
                HttpContext.Session.SetInt32("UserId", user.AccountId);
                return RedirectToPage("/BookStore/Books/Index");
            }
            return Page();

        }
    }
}
