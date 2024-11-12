using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DemoPRN1.Pages.Custommer
{
    public class ListBookByBookstoreModel : PageModel
    {
		private readonly PJPRN221Context _context;

		public ListBookByBookstoreModel(PJPRN221Context context)
		{
			_context = context;
		}
		public Account accounts { get; set; }
		public List<Category> Categories { get; set; }
		public List<Book> Books { get; set; }
		public PagingInfo PagingInfo { get; set; }
		public int CurrentCategoryId { get; set; }
		public int BookStoreId { get; set; }
		public string StoreName { get; set; }
		public int CurrentPage { get; set; }
		public async Task<IActionResult> OnPostAddToCart(int bookId, int? categoryId, int pageNumber, string type, int bookStoreId, string bookStoreName)
		{
			// Lấy thông tin giỏ hàng từ Session
			var cart = HttpContext.Session.GetString("addCart");
			Dictionary<string, int> cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);

			// Tạo khóa để lưu loại sách và số lượng vào giỏ hàng
			string cartTypeAndName = bookId.ToString() + "_" + type;
			if (cartItems.ContainsKey(cartTypeAndName))
			{
				cartItems[cartTypeAndName]++;
			}
			else
			{
				cartItems[cartTypeAndName] = 1;
			}

			// Cập nhật giỏ hàng trong Session
			HttpContext.Session.SetString("addCart", JsonConvert.SerializeObject(cartItems));
			TempData["CartCount"] = cartItems.Count;

			// Truyền bookStoreId khi gọi OnGetAsync
			return await OnGetAsync(bookStoreId, categoryId, searchString: "", pageNumber);
		}

		public async Task<IActionResult> OnGetAsync(int bookStoreId, int? categoryId = null, string searchString = "", int pageNumber = 1)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");
			if (userId != null)
			{
				accounts = _context.Accounts.FirstOrDefault(x => x.AccountId == userId.Value);
			}
			int pageSize = 8;
			Categories = await _context.Categories.ToListAsync();

			// Khởi tạo truy vấn sách với điều kiện lọc theo bookStoreId
			IQueryable<Book> query = _context.Books
											 .Include(b => b.Category)
											 .Where(b => b.BookStoreId == bookStoreId);
			BookStoreId = bookStoreId;

			if (bookStoreId == 2) { 
				StoreName = "Nhà Xuất Bản Tổng hợp TP.Hà Nội";
			}

			// Lọc theo thể loại nếu có
			if (categoryId.HasValue && categoryId.Value != 0)
			{
				query = query.Where(b => b.CategoryId == categoryId.Value && b.BookStoreId == bookStoreId);
				CurrentCategoryId = categoryId.Value;
			}

			// Tìm kiếm sách theo tên nếu có
			if (!string.IsNullOrEmpty(searchString))
			{
				query = query.Where(b => b.CategoryId == categoryId.Value && b.BookStoreId == bookStoreId);
			}

			int totalBooks = await query.CountAsync();
			PagingInfo = new PagingInfo
			{
				CurrentPage = pageNumber,
				TotalItems = totalBooks,
				ItemsPerPage = pageSize,
			};
			CurrentPage = pageNumber;

			// Lấy thông tin giỏ hàng từ Session
			var cart = HttpContext.Session.GetString("addCart");
			Dictionary<string, int> cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);
			TempData["CartCount"] = cartItems.Count;

			// Phân trang
			Books = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
			return Page();
		}
	}
}

