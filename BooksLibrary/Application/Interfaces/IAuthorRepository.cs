using BooksLibrary.Application.DTOs;
using BooksLibrary.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.Application.Interfaces
{
    public interface IAuthorRepository
    {
        Task<List<AuthorDto>> GetAllAsync(CancellationToken ct = default);
        Task<Author?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Author author, CancellationToken ct = default);
        Task UpdateAsync(Author author, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
