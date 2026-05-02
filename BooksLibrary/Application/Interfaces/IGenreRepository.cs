using BooksLibrary.Application.DTOs;
using BooksLibrary.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.Application.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<GenreDto>> GetAllAsync(CancellationToken ct = default);
        Task<Genre?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Genre genre, CancellationToken ct = default);
        Task UpdateAsync(Genre genre, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
