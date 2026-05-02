using BooksLibrary.Application.DTOs;
using BooksLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using BooksLibrary.Infrastructure.Data;
using BooksLibrary.Application.Interfaces;

namespace BooksLibrary.Infrastructure.Data.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public LoanRepository(IDbContextFactory<AppDbContext> factory)
            => _factory = factory;

        public async Task<List<LoanDto>> GetActiveAsync(CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Loans
                .AsNoTracking()
                .Where(l => l.ReturnedAt == null)
                .Include(l => l.Book)
                .Include(l => l.Reader)
                .OrderBy(l => l.DueDate)
                .Select(l => new LoanDto
                {
                    Id = l.Id,
                    BookTitle = l.Book.Title,
                    ReaderName = l.Reader.FullName,
                    LoanedAt = l.LoanedAt.ToString("dd.MM.yyyy"),
                    DueDate = l.DueDate.ToString("dd.MM.yyyy"),
                    ReturnedAt = null,
                    IsOverdue = DateTime.UtcNow > l.DueDate,
                    IsReturned = false
                })
                .ToListAsync(ct);
        }

        public async Task<List<LoanDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Loans
                .AsNoTracking()
                .Include(l => l.Book)
                .Include(l => l.Reader)
                .OrderByDescending(l => l.LoanedAt)
                .Select(l => new LoanDto
                {
                    Id = l.Id,
                    BookTitle = l.Book.Title,
                    ReaderName = l.Reader.FullName,
                    LoanedAt = l.LoanedAt.ToString("dd.MM.yyyy"),
                    DueDate = l.DueDate.ToString("dd.MM.yyyy"),
                    ReturnedAt = l.ReturnedAt == null ? null : ((DateTime)l.ReturnedAt!).ToString("dd.MM.yyyy"),
                    IsOverdue = l.ReturnedAt == null && DateTime.UtcNow > l.DueDate,
                    IsReturned = l.ReturnedAt != null
                })
                .ToListAsync(ct);
        }

        // Выдать книгу
        public async Task LoanBookAsync(int bookId, int readerId,
                                        int days, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);

            // Проверяем наличие свободных экземпляров
            var borrowed = await ctx.Loans
                .CountAsync(l => l.BookId == bookId && l.ReturnedAt == null, ct);
            var book = await ctx.Books.FindAsync([bookId], ct)
                ?? throw new InvalidOperationException("Книга не найдена");

            if (borrowed >= book.Quantity)
                throw new InvalidOperationException("Нет свободных экземпляров");

            ctx.Loans.Add(new Loan
            {
                BookId = bookId,
                ReaderId = readerId,
                LoanedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(days)
            });
            await ctx.SaveChangesAsync(ct);
        }

        // Вернуть книгу
        public async Task ReturnBookAsync(int loanId, CancellationToken ct = default)
        {
            await using var ctx = await _factory.CreateDbContextAsync(ct);
            var loan = await ctx.Loans.FindAsync([loanId], ct)
                ?? throw new InvalidOperationException("Выдача не найдена");

            loan.ReturnedAt = DateTime.UtcNow;
            await ctx.SaveChangesAsync(ct);
        }
    }
}
