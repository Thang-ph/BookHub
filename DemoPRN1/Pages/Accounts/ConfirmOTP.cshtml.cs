using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using static System.Net.WebRequestMethods;



namespace DemoPRN1.Pages.Accounts
{
    public class ConfirmOTPModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public ConfirmOTPModel(PJPRN221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public OTPInputModel Input { get; set; }

        public class OTPInputModel
        {
            [Required(ErrorMessage = "OTP không được bỏ trống")]
            public string ReOTP { get; set; }
            public string OTP { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
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


            if (!Input.ReOTP.Equals(Input.OTP))
            {
                ModelState.AddModelError(string.Empty, "Sai mã OTP");
                return Page();
            }
            Account newAccount = new Account()
            {
                Email = Input.Email,
                Password = Input.Password,
                FullName = Input.FullName,
                PhoneNumer = Input.PhoneNumber,
                Address = Input.Address,
                Status = true,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                RoleId = 2
            };
            _context.Add(newAccount);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Accounts/Login");
        }      
    }
}
