using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.Infrastructure.Data.Repositories
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public ReaderRepository(IDbContextFactory<AppDbContext> factory)
            => _factory = factory;

        public async Task<List<ReaderDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Readers
                .AsNoTracking()
                .OrderBy(r => r.FullName)
                .Select(r => new ReaderDto
                {
                    Id = r.Id,
                    FullName = r.FullName,
                    Phone = r.Phone,
                    Email = r.Email
                })
                .ToListAsync(ct);
        }

        public async Task<Reader?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Readers.FindAsync(new object[] { id }, ct);
        }

        public async Task AddAsync(Reader reader, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            ctx.Readers.Add(reader);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Reader reader, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            ctx.Readers.Update(reader);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            await ctx.Readers.Where(r => r.Id == id).ExecuteDeleteAsync(ct);
        }
    }
}
