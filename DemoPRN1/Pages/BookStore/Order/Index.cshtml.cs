using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace DemoPRN1.Pages.BookStore.Order;


public class Index : PageModel
{
    private readonly PJPRN221Context _context;
    public List<BookWithQuantityViewModel> Order { get; set; } = new List<BookWithQuantityViewModel>();

    public List<Oderdetail> OderDetails { get; set; } = new List<Oderdetail>();
    public class BookWithQuantityViewModel
    {
        public Book Book { get; set; }
        public Oder Order { get; set; }
        public Oderdetail Oderdetail { get; set; }
    }

    public Index(PJPRN221Context context)
    {
        _context = context;
    }
    
    public void OnGet()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        GetBookOfUser();
    }
    private void GetBookOfUser()
    {
        Order = (from o in _context.Oders
                join od in _context.Oderdetails on o.OrderdetailsId equals od.OrderdetailsId
                join b in _context.Books on od.BookId equals b.BookId
                where o.Status == 0
                orderby o.UpdateAt descending
                select new BookWithQuantityViewModel
                {
                    Book = b,
                    Order = o,
                    Oderdetail = od
                })
            .ToList();
    }
    public IActionResult OnPost(int id)
    {
        var Order = _context.Oders.FirstOrDefault(b => b.OrderId == id);
        if (Order == null)
        {
            return NotFound();
        }
        Order.Status = 1;
        _context.Update(Order);
        _context.SaveChanges(); 

        return RedirectToPage("/BookStore/Order/Index");
    }
}