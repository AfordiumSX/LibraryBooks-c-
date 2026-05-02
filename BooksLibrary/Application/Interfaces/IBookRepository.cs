using BooksLibrary.Application.DTOs;
using BooksLibrary.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<List<BookDto>> GetAllAsync(CancellationToken ct = default);
        Task<List<BookDto>> SearchAsync(string term, CancellationToken ct = default);
        Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Book book, CancellationToken ct = default);
        Task UpdateAsync(Book book, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}