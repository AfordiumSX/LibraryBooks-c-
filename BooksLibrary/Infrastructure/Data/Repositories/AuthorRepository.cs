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
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public AuthorRepository(IDbContextFactory<AppDbContext> factory)
            => _factory = factory;

        public async Task<List<AuthorDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Authors
                .AsNoTracking()
                .OrderBy(a => a.LastName)
                .Select(a => new AuthorDto
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName
                })
                .ToListAsync(ct);
        }

        public async Task<Author?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Authors.FindAsync(new object[] { id }, ct);
        }

        public async Task AddAsync(Author author, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            ctx.Authors.Add(author);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Author author, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            ctx.Authors.Update(author);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            await ctx.Authors.Where(a => a.Id == id).ExecuteDeleteAsync(ct);
        }
    }
}
