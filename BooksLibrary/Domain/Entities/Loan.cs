using System;
using System.Collections.Generic;
using System.Text;

namespace BooksLibrary.Domain.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int ReaderId { get; set; }
        public DateTime LoanedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public Book Book { get; set; } = null!;
        public Reader Reader { get; set; } = null!;

        public bool IsOverdue => ReturnedAt == null && DateTime.UtcNow > DueDate;
        public bool IsReturned => ReturnedAt != null;
    }
}
