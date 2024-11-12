using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoPRN1.Pages.Custommer
{
    public class LogoutModel : PageModel
    {
       
		public async Task<IActionResult> OnGet()
		{

			HttpContext.Session.Clear();
			return RedirectToPage("/Accounts/Login");
		}
	}
}
