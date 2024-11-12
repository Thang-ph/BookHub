using System;
using System.Collections.Generic;

namespace DemoPRN1.Models
{
    public partial class Bookstore
    {
        public Bookstore()
        {
            Books = new HashSet<Book>();
        }

        public int BookStoreId { get; set; }
        public string? StoreName { get; set; }
        public string? IdentityImg { get; set; }
        public string? BankNumber { get; set; }
        public string? BankName { get; set; }
        public bool? Status { get; set; }
        public string? Address { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
