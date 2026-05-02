using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.ViewModels.Loans
{
    public class NewLoanViewModel : ReactiveObject
    {
        private readonly ILoanRepository _loanRepo;
        private readonly IBookRepository _bookRepo;
        private readonly IReaderRepository _readerRepo;

        [Reactive] public BookDto? SelectedBook { get; set; }
        [Reactive] public ReaderDto? SelectedReader { get; set; }
        [Reactive] public int Days { get; set; } = 14;
        [Reactive] public string? ErrorMessage { get; set; }

        public ObservableCollection<BookDto> Books { get; } = new();
        public ObservableCollection<ReaderDto> Readers { get; } = new();

        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        private ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public Action<bool>? RequestClose { get; set; }

        public NewLoanViewModel(
            ILoanRepository loanRepo,
            IBookRepository bookRepo,
            IReaderRepository readerRepo)
        {
            _loanRepo = loanRepo;
            _bookRepo = bookRepo;
            _readerRepo = readerRepo;

            var canConfirm = this.WhenAnyValue(
                x => x.SelectedBook,
                x => x.SelectedReader,
                (b, r) => b != null && r != null);

            ConfirmCommand = ReactiveCommand.CreateFromTask(ConfirmAsync, canConfirm);
            CancelCommand = ReactiveCommand.Create(() => RequestClose?.Invoke(false));
            LoadCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
            LoadCommand.Execute().Subscribe();
        }

        private async Task LoadDataAsync(CancellationToken ct)
        {
            var books = await _bookRepo.GetAllAsync(ct);
            var readers = await _readerRepo.GetAllAsync(ct);

            Books.Clear();
            foreach (var b in books) Books.Add(b);

            Readers.Clear();
            foreach (var r in readers) Readers.Add(r);
        }

        private async Task ConfirmAsync(CancellationToken ct)
        {
            if (SelectedBook is null || SelectedReader is null) return;

            try
            {
                ErrorMessage = null;
                await _loanRepo.LoanBookAsync(
                    SelectedBook.Id,
                    SelectedReader.Id,
                    Days,
                    ct);
                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
