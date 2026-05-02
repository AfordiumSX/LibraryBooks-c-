using System;
using System.Collections.Generic;
using System.Text;

namespace BooksLibrary.Application.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string ReaderName { get; set; } = string.Empty;
        public string LoanedAt { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string? ReturnedAt { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsReturned { get; set; }
    }
}
