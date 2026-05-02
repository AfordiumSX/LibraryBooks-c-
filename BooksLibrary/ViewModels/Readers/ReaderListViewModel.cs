using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Views;
using BooksLibrary.Views.Readers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.ViewModels.Readers
{
    public class ReaderListViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IReaderRepository _repo;

        [Reactive] public ReaderDto? SelectedReader { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        [Reactive] public string? ErrorMessage { get; set; }

        public ObservableCollection<ReaderDto> Readers { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public ReaderListViewModel(IReaderRepository repo, IScreen screen)
        {
            _repo = repo;
            HostScreen = screen;

            var hasSelection = this.WhenAnyValue(x => x.SelectedReader)
                .Select(r => r is not null);

            LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync);
            AddCommand = ReactiveCommand.CreateFromTask(NavigateToAddAsync);
            EditCommand = ReactiveCommand.CreateFromTask(NavigateToEditAsync, hasSelection);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync, hasSelection);

            LoadCommand.Execute().Subscribe();
        }

        private async Task LoadAsync(CancellationToken ct)
        {
            IsLoading = true;
            try
            {
                ErrorMessage = null;
                var items = await _repo.GetAllAsync(ct);
                Readers.Clear();
                foreach (var r in items) Readers.Add(r);
            }
            finally { IsLoading = false; }
        }

        private async Task DeleteAsync(CancellationToken ct)
        {
            if (SelectedReader is null) return;
            try
            {
                ErrorMessage = null;
                await _repo.DeleteAsync(SelectedReader.Id, ct);
                Readers.Remove(SelectedReader);
                SelectedReader = null;
            }
            catch (Exception)
            {
                ErrorMessage = "Невозможно удалить читателя у которого есть история одолженных книг.";
            }
        }

        private async Task NavigateToAddAsync(CancellationToken ct)
        {
            var vm = new ReaderEditViewModel(_repo, null);
            var window = new ReaderEditView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        private async Task NavigateToEditAsync(CancellationToken ct)
        {
            if (SelectedReader is null) return;
            var vm = new ReaderEditViewModel(_repo, SelectedReader);
            var window = new ReaderEditView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        public IScreen HostScreen { get; }
        public string UrlPathSegment => "readers";
    }
}
