using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DemoPRN1.Pages.Custommer
{
	public class ListBookModel : PageModel
	{
		private readonly PJPRN221Context _context;

		public ListBookModel(PJPRN221Context context)
		{
			_context = context;
		}

		public Account accounts { get; set; }

		public List<Category> Categories { get; set; }
		public List<Book> Books { get; set; }
		public PagingInfo PagingInfo { get; set; }
		public int CurrentCategoryId { get; set; }

		public int CurrentPage { get; set; }

		public async Task<IActionResult> OnPostAddToCart(int bookId, int categoryId, int pageNumber, string type)
		{
			var cart = HttpContext.Session.GetString("addCart");

			// Lấy thông tin giỏ hàng từ Session
			Dictionary<string, int> cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);

			string cartTypeAndName = bookId.ToString() + "_" + type;
			if (cartItems.ContainsKey(cartTypeAndName))
			{
				cartItems[cartTypeAndName]++;
			}
			else
			{
				cartItems[cartTypeAndName] = 1;
			}

			HttpContext.Session.SetString("addCart", JsonConvert.SerializeObject(cartItems));

			TempData["CartCount"] = cartItems.Count;

			return await OnGetAsync(null, "", pageNumber);
		}


		public async Task<IActionResult> OnGetAsync(int? categoryId, string searchString, int pageNumber = 1)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");
			accounts = _context.Accounts.FirstOrDefault(x => x.AccountId == userId.Value);
			int pageSize = 8;
			Categories = await _context.Categories.ToListAsync();

			IQueryable<Book> query = _context.Books.Include(b => b.Category);

			// L?c theo th? lo?i
			if (categoryId.HasValue && categoryId != 0)
			{
				query = query.Where(b => b.CategoryId == categoryId);
				CurrentCategoryId = categoryId.Value;
			}

			// Tìm ki?m sách theo tên
			if (!string.IsNullOrEmpty(searchString))
			{
				query = query.Where(b => b.BookTitle.Contains(searchString) || b.Author.Contains(searchString));
			}

			int totalBooks = await query.CountAsync();
			PagingInfo = new PagingInfo
			{
				CurrentPage = pageNumber,
				TotalItems = totalBooks,
				ItemsPerPage = pageSize,
			};
			CurrentPage = pageNumber;

			//phần session để lưu Cart
			var cart = HttpContext.Session.GetString("addCart");
			Dictionary<string, int> cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);
			TempData["CartCount"] = cartItems.Count;


			Books = await query.Where(X => X.Status == true).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
			return Page();
		}
	}
}

public class PagingInfo
{
	public int TotalItems { get; set; }
	public int ItemsPerPage { get; set; }
	public int CurrentPage { get; set; }
	public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
}
