using DemoPRN1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoPRN1.Pages.Custommer
{
    public class PurchaseModel : PageModel
    {
        private readonly PJPRN221Context _context;
        public List<BookWithQuantityViewModel> BookOfUser { get; set; } = new List<BookWithQuantityViewModel>();
        public List<BookWithQuantityViewModel> BookBuy { get; set; } = new List<BookWithQuantityViewModel>();
        public List<BookWithQuantityViewModel> BookRent { get; set; } = new List<BookWithQuantityViewModel>();
        public List<Oderdetail> OderDetails { get; set; } = new List<Oderdetail>();
        public Account accounts { get; set; }


        public class BookWithQuantityViewModel
        {
            public Book Book { get; set; }
            public Oder Order { get; set; }
            public Oderdetail Oderdetail { get; set; }
        }

        public PurchaseModel(PJPRN221Context context)
        {
            _context = context;
        }

        public void OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            accounts = _context.Accounts.FirstOrDefault(x => x.AccountId == userId.Value);
            GetBookOfUser(userId);
            GetBookBuy(userId);
            GetRentBook(userId);
        }


        private void GetBookOfUser(int? userId)
        {
            BookOfUser = (from o in _context.Oders
                          join od in _context.Oderdetails on o.OrderdetailsId equals od.OrderdetailsId
                          join b in _context.Books on od.BookId equals b.BookId
                          where o.AccountId == userId
                          orderby o.UpdateAt descending
                          select new BookWithQuantityViewModel
                          {
                              Book = b,
                              Order = o,
                              Oderdetail = od
                          })
                 .ToList();
        }
        private void GetBookBuy(int? userId)
        {
            BookBuy = (from o in _context.Oders
                       join od in _context.Oderdetails on o.OrderdetailsId equals od.OrderdetailsId
                       join b in _context.Books on od.BookId equals b.BookId
                       where o.AccountId == userId && od.Type == true
                       orderby o.UpdateAt descending
                       select new BookWithQuantityViewModel
                       {
                           Book = b,
                           Order = o,
                           Oderdetail = od
                       })
                .ToList();
        }
        private void GetRentBook(int? userId)
        {
            BookRent = (from o in _context.Oders
                        join od in _context.Oderdetails on o.OrderdetailsId equals od.OrderdetailsId
                        join b in _context.Books on od.BookId equals b.BookId
                        where o.AccountId == userId && od.Type == false
                        orderby o.UpdateAt descending
                        select new BookWithQuantityViewModel
                        {
                            Book = b,
                            Order = o,
                            Oderdetail = od
                        })
                .ToList();
        }
    }
}
