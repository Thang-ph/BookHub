using System;
using System.Collections.Generic;

namespace DemoPRN1.Models
{
    public partial class Account
    {
        public Account()
        {
            Bookstores = new HashSet<Bookstore>();
            Oders = new HashSet<Oder>();
        }

        public int AccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumer { get; set; }
        public string? Address { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Bookstore> Bookstores { get; set; }
        public virtual ICollection<Oder> Oders { get; set; }
    }
}
