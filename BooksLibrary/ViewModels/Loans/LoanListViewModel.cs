using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Views;
using BooksLibrary.Views.Loans;
using Microsoft.Extensions.DependencyInjection;
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
    public class LoanListViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly ILoanRepository _loanRepo;

        [Reactive] public LoanDto? SelectedLoan { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        [Reactive] public bool ShowAll { get; set; }
        [Reactive] public string? ErrorMessage { get; set; }

        public ObservableCollection<LoanDto> Loans { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> ReturnCommand { get; }
        public ReactiveCommand<Unit, Unit> NewLoanCommand { get; }

        public LoanListViewModel(ILoanRepository loanRepo, IScreen screen)
        {
            _loanRepo = loanRepo;
            HostScreen = screen;

            var canReturn = this.WhenAnyValue(x => x.SelectedLoan)
                .Select(l => l is { IsReturned: false });

            LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync);
            ReturnCommand = ReactiveCommand.CreateFromTask(ReturnAsync, canReturn);
            NewLoanCommand = ReactiveCommand.CreateFromTask(NavigateToNewLoanAsync);

            this.WhenAnyValue(x => x.ShowAll)
                .Skip(1)
                .Select(_ => Unit.Default)
                .InvokeCommand(LoadCommand);

            LoadCommand.Execute().Subscribe();
        }

        private async Task LoadAsync(CancellationToken ct)
        {
            IsLoading = true;
            try
            {
                ErrorMessage = null;
                var items = ShowAll
                    ? await _loanRepo.GetAllAsync(ct)
                    : await _loanRepo.GetActiveAsync(ct);
                Loans.Clear();
                foreach (var l in items) Loans.Add(l);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally { IsLoading = false; }
        }

        private async Task ReturnAsync(CancellationToken ct)
        {
            if (SelectedLoan is null) return;
            try
            {
                ErrorMessage = null;
                await _loanRepo.ReturnBookAsync(SelectedLoan.Id, ct);
                await LoadAsync(ct);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private async Task NavigateToNewLoanAsync(CancellationToken ct)
        {
            var vm = new NewLoanViewModel(
                _loanRepo,
                App.Services.GetRequiredService<IBookRepository>(),
                App.Services.GetRequiredService<IReaderRepository>());
            var window = new NewLoanView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        public IScreen HostScreen { get; }
        public string UrlPathSegment => "loans";
    }
}
