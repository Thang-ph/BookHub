using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoPRN1.Pages.Accounts
{
    public class ManageAccountModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public ManageAccountModel(PJPRN221Context context)
        {
            _context = context;

        }
        [BindProperty]
        public List<Account> accounts {  get; set; }

        public IActionResult OnGet()
        {
            accounts=_context.Accounts.Include(x => x.Role).ToList();
            return Page();
        }
        public IActionResult OnPostUnban(int accountId)
        {
            Account account=_context.Accounts.FirstOrDefault(x => x.AccountId == accountId);
            account.Status = true;
            _context.Update(account);
            _context.SaveChanges();
            return RedirectToPage(); 
        }

        public IActionResult OnPostBan(int accountId)
        {
            Account account = _context.Accounts.FirstOrDefault(x => x.AccountId == accountId);
            account.Status = false;
            _context.Update(account);
            _context.SaveChanges();
            return RedirectToPage(); 
        }
    }
}
