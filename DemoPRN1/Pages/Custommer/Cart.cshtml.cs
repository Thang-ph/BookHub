using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using Net.payOS;
using Newtonsoft.Json;

namespace DemoPRN1.Pages.Custommer
{
    public class CartModel : PageModel
    {
        private readonly PJPRN221Context _context;

        public CartModel(PJPRN221Context context)
        {
            _context = context;
        }
        [BindProperty]
        public List<Oderdetail> oderdetail { get; set; } = new List<Oderdetail>();
        public Account accounts { get; set; }
        // CartItem với cartItem là 2 cái khác nhau
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public async Task<IActionResult> OnGetAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            accounts = _context.Accounts.FirstOrDefault(x => x.AccountId == userId.Value);
            var cart = HttpContext.Session.GetString("addCart");
            Dictionary<string, int> cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);
            TempData["CartCount"] = cartItems.Count;

            using (var _context = new PJPRN221Context())
            {
                foreach (KeyValuePair<string, int> item in cartItems)
                {
                    int bookId = int.Parse(item.Key.Split("_")[0]);
                    string type = item.Key.Split('_')[1];

                    Book book = await _context.Books
                        .Include(b => b.Category) // Chỉ nạp Category, không nạp Books trong Category
                        .FirstOrDefaultAsync(b => b.BookId == bookId);

                    // Không nạp Category.Books để tránh chu kỳ
                    if (book != null)
                    {
                        book.Category.Books = null; // Đảm bảo không có chu kỳ khi serialize
                        Items.Add(new CartItem
                        {
                            book = book,
                            quantity = item.Value,
                            type = type
                        });
                    }
                }
            }

            return Page();
        }
        [HttpPost]
        public async Task<IActionResult> OnPostCheckoutAsync(decimal total)
        {
            List<CartItem> Items2 = new List<CartItem>();
            int? userId = HttpContext.Session.GetInt32("UserId");
            var cart = HttpContext.Session.GetString("addCart");
            Dictionary<string, int> cartItems = string.IsNullOrEmpty(cart) ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(cart);
            TempData["CartCount"] = cartItems.Count;

            using (var _context = new PJPRN221Context())
            {
                foreach (KeyValuePair<string, int> item in cartItems)
                {
                    int bookId = int.Parse(item.Key.Split("_")[0]);
                    string type = item.Key.Split('_')[1];

                    Book book = await _context.Books
                        .Include(b => b.Category) // Chỉ nạp Category, không nạp Books trong Category
                        .FirstOrDefaultAsync(b => b.BookId == bookId);

                    // Không nạp Category.Books để tránh chu kỳ
                    if (book != null)
                    {
                        book.Category.Books = null; // Đảm bảo không có chu kỳ khi serialize
                        Items2.Add(new CartItem
                        {
                            book = book,
                            quantity = item.Value,
                            type = type
                        });
                    }
                }
            }
            foreach (var item in Items2)
            {

                Oderdetail orDetail = new Oderdetail();


                if (item.type.Equals("rent"))
                {
                    orDetail.Type = false;
                    orDetail.Startdate = item.startDate;
                    orDetail.EndDate = item.endDate;

                }
                else
                {
                    orDetail.Type = true;

                }
                orDetail.BookId = item.book.BookId;
                orDetail.Quantity = item.quantity;

                _context.Oderdetails.Add(orDetail);
                _context.SaveChanges();

                Oder or = new Oder();
                or.Status = 0;
                or.CreateAt = DateTime.Now;
                or.UpdateAt = DateTime.Now;
                or.AccountId = userId.Value;
                or.OrderdetailsId = orDetail.OrderdetailsId;
                _context.Oders.Add(or);
                _context.SaveChanges();
            }

            
           var clientId = "57eab08d-55fa-4015-87e8-6d4fb8405ae6";
            var apiKey = "b127e565-3917-4aeb-af8c-787a5ae3e181";
        var checksumKey = "6604357da9c0dad40f11c09f29424394808cb23b921871f5136de5d5f14fc606";

        var payOS = new PayOS(clientId, apiKey, checksumKey);

        var domain = "https://localhost:7048";


        var paymentLinkRequest = new PaymentData(
            orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
            amount: (int)total,
            description: "Thanh toan don hang",
              items: [new("Mì tôm hảo hảo ly", 1, 2000)],
            returnUrl: domain + "/Custommer/Home",
            cancelUrl: domain + "/Error"
        );
        var response = await payOS.createPaymentLink(paymentLinkRequest);

        Response.Headers.Append("Location", response.checkoutUrl);
            return new StatusCodeResult(303);
    }


    


        // Handler cho yêu cầu AJAX, sử dụng phương thức POST xử lí cho thay đôi input type number
        public JsonResult OnPostUpdateQuantity([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return new JsonResult(new { Message = "Dữ liệu không hợp lệ" }) { StatusCode = 400 };
            }

            int bookId = int.Parse(data.Split("_")[0]);
            string type = data.Split("_")[1];
            int quantity = int.Parse(data.Split("_")[2]);

            foreach (var item in Items)
            {
                if (item.book.BookId == bookId && item.type == type)
                {
                    item.quantity = quantity;
                }
                else
                {
                    continue;
                }
            }

            return new JsonResult(new { Items });
        }

    }
}

public class CartItem
{
    public Book book { get; set; }
    public int quantity { get; set; }
    public string type { get; set; }

    public DateTime? startDate { get; set; } = DateTime.Today;
    public DateTime? endDate { get; set; } = DateTime.Today.AddDays(1);

}
