using System;
using System.Collections.Generic;

namespace DemoPRN1.Models
{
    public partial class Book
    {
        public Book()
        {
            Bookratings = new HashSet<Bookrating>();
            Oderdetails = new HashSet<Oderdetail>();
        }

        public int BookId { get; set; }
        public string? Image { get; set; }
        public string? BookTitle { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string? Isbn { get; set; }
        public decimal? RentalPrice { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public bool? Status { get; set; }
        public int? RentalQuantity { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? BookStoreId { get; set; }
        public int? CategoryId { get; set; }

        public virtual Bookstore? BookStore { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Bookrating> Bookratings { get; set; }
        public virtual ICollection<Oderdetail> Oderdetails { get; set; }
    }
}
