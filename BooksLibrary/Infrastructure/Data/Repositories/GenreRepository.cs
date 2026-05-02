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
    public class GenreRepository : IGenreRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public GenreRepository(IDbContextFactory<AppDbContext> factory)
            => _factory = factory;

        public async Task<List<GenreDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Genres
                .AsNoTracking()
                .OrderBy(g => g.Name)
                .Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync(ct);
        }

        public async Task<Genre?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Genres.FindAsync(new object[] { id }, ct);
        }

        public async Task AddAsync(Genre genre, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            ctx.Genres.Add(genre);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Genre genre, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            ctx.Genres.Update(genre);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            await ctx.Genres.Where(g => g.Id == id).ExecuteDeleteAsync(ct);
        }
    }
}
