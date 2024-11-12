using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoPRN1.Pages.Accounts
{
    public class AddAdminModel : PageModel
    {
        public PJPRN221Context context=new PJPRN221Context();

        [BindProperty]
        public Account Account { get; set; }=new Account();
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Account.RoleId = 1;
            Account.Status = true;
            Account.CreateAt = DateTime.Now;    
            Account.UpdateAt = DateTime.Now;
            context.Accounts.Add(Account); 
            context.SaveChanges();
            return RedirectToPage("/Accounts/ManageAccount"); 
        }
    }
}
