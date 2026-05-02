using BooksLibrary.Application.DTOs;
using BooksLibrary.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.Application.Interfaces
{
    public interface IReaderRepository
    {
        Task<List<ReaderDto>> GetAllAsync(CancellationToken ct = default);
        Task<Reader?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Reader reader, CancellationToken ct = default);
        Task UpdateAsync(Reader reader, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
