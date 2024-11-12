using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;


namespace DemoPRN1.Pages.Accounts
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public ForgotPasswordModel(PJPRN221Context context)
        {
            _context = context;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public bool LoginFailed { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

           
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
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == Input.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại!");
                return Page();
            }

            
            return RedirectToPage("/Accounts/ConfirmOTP");

        }
        public string GenerateOTP()
        {
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            return otp;
        }
        public async Task SendOTPEmail(string email, string otp)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("trungvdhe172137@fpt.edu.vn", "gepo wuqh elwj ouvb"),
                EnableSsl = true,
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("trungvdhe172137@fpt.edu.vn"),
                Subject = "Confirm your email with OTP",
                Body = $"Your OTP code is: {otp}",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
