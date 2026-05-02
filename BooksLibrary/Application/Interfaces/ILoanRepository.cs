using BooksLibrary.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.Application.Interfaces
{
    public interface ILoanRepository
    {
        Task<List<LoanDto>> GetActiveAsync(CancellationToken ct = default);
        Task<List<LoanDto>> GetAllAsync(CancellationToken ct = default);
        Task LoanBookAsync(int bookId, int readerId, int days, CancellationToken ct = default);
        Task ReturnBookAsync(int loanId, CancellationToken ct = default);
    }
}
