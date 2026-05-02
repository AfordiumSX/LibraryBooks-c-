using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BooksLibrary.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Quantity { get; set; } 
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public Author Author { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public int BorrowedCount => Loans.Count(l => l.ReturnedAt == null);
        public int AvailableCount => Quantity - BorrowedCount;
    }
}
