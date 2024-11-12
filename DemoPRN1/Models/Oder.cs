using System;
using System.Collections.Generic;

namespace DemoPRN1.Models
{
    public partial class Oder
    {
        public int OrderId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? AccountId { get; set; }
        public int? OrderdetailsId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Oderdetail? Orderdetails { get; set; }
    }
}
