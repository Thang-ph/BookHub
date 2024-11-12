using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DemoPRN1.Pages.Custommer
{
    public class ProfileModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public ProfileModel(PJPRN221Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;

        }
        [BindProperty]
        public Account accounts { get; set; } 
        [BindProperty]
        public InputPassword pass { get; set; }
        public class InputPassword
        {
            [Required(ErrorMessage = "Vui lòng nhập mật khẩu Hiện tại.")]
            [DataType(DataType.Password)]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.", MinimumLength = 6)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Bạn cần xác minh lại mật khẩu.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            public string ConfirmPassword { get; set; }

        }
        public async Task LoadProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                accounts = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == userId.Value);
            }
            else
            {
                accounts = null; 
            }
        }
        public async Task<IActionResult> OnGetAsync()
        {

			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId.HasValue)
			{
				accounts = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == userId.Value);
			}
			else
			{
				accounts = null;
			}
			return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadProfile();
                return Page();
            }
            int? userId = HttpContext.Session.GetInt32("UserId");
            accounts = _context.Accounts.FirstOrDefault(x => x.AccountId == userId.Value);
            if (!accounts.Password.Equals(pass.OldPassword)) {
                await LoadProfile();
                ModelState.AddModelError(string.Empty, "Sai mật khẩu hiện tại");
                return Page();
            }
            await LoadProfile();
            accounts.Password = pass.Password;
            accounts.UpdateAt = DateTime.Now;
            _context.Accounts.Update(accounts);
            _context.SaveChanges();
            LoadProfile(); 
            return Page();
        }
    }
}
