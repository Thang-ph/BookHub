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
    public class RegisterModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public RegisterModel(PJPRN221Context context)
        {
            _context = context;
        }
        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public bool LoginFailed { get; set; }
        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Email không được bỏ trống")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Mật khẩu phải dủ 6 ký tự.", MinimumLength = 6)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Bạn Cần xác minh lại mật khẩu")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Họ và tên không được bỏ trống")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Số diện thoại không được bỏ trống")]
            [Phone(ErrorMessage = "Số diện thoại không hợp lệ")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
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
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == Input.Email);

            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "Email đã được sử dụng."); 
                return Page(); 
            }
            String OTP = GenerateOTP(); 
            String newEmail = Input.Email ?? string.Empty; 
            String newPassword = Input.Password ?? string.Empty;
            String newFullName = Input.FullName ?? string.Empty;
            String newPhoneNumber = Input.PhoneNumber ?? string.Empty;
            String newAddress = Input.Address ?? string.Empty;

          
            SendOTPEmail(Input.Email, OTP);

            // Lưu các giá trị vào TempData để sử dụng ở trang ConfirmOTP
            TempData["Otp"] = OTP;
            TempData["newEmail"] = newEmail;
            TempData["newPassword"] = newPassword;
            TempData["newFullName"] = newFullName;
            TempData["newPhoneNumber"] = newPhoneNumber;
            TempData["newAddress"] = newAddress;

            return  RedirectToPage("/Accounts/ConfirmOTP");

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
