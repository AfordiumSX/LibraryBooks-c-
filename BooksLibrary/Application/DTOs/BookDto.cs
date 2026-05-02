using System;
using System.Collections.Generic;
using System.Text;

namespace BooksLibrary.Application.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Quantity { get; set; }
        public int AvailableCount { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string GenreName { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
    }
}
