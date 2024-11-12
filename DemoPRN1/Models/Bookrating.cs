

using System;
using System.Collections.Generic;

namespace DemoPRN1.Models
{
    public partial class Bookrating
    {
        public int RatingId { get; set; }
        public string? Comment { get; set; }
        public int? BookId { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Book? Book { get; set; }
    }
}
