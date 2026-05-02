using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Domain.Entities;
using BooksLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.Infrastructure.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public BookRepository(IDbContextFactory<AppDbContext> factory)
            => _factory = factory;

        public async Task<List<BookDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .OrderBy(b => b.Title)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    Year = b.Year,
                    Quantity = b.Quantity,
                    AuthorId = b.AuthorId,
                    GenreId = b.GenreId,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName,
                    GenreName = b.Genre.Name,
                    AvailableCount = b.Quantity - b.Loans.Count(l => l.ReturnedAt == null)
                })
                .ToListAsync(ct);
        }

        public async Task<List<BookDto>> SearchAsync(string term, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            var lower = term.ToLower();
            return await ctx.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Where(b => b.Title.ToLower().Contains(lower)
                         || b.Author.LastName.ToLower().Contains(lower)
                         || b.ISBN.Contains(term))
                .OrderBy(b => b.Title)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    Year = b.Year,
                    Quantity = b.Quantity,
                    AuthorId = b.AuthorId,
                    GenreId = b.GenreId,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName,
                    GenreName = b.Genre.Name,
                    AvailableCount = b.Quantity - b.Loans.Count(l => l.ReturnedAt == null)
                })
                .ToListAsync(ct);
        }

        public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Books.FindAsync(new object[] { id }, ct);
        }

        public async Task AddAsync(Book book, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            ctx.Books.Add(book);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Book book, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            ctx.Books.Update(book);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            await ctx.Books.Where(b => b.Id == id).ExecuteDeleteAsync(ct);
        }
    }
}