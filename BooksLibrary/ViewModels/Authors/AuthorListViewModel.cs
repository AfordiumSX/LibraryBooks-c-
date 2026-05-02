using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Views;
using BooksLibrary.Views.Authors;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.ViewModels.Authors
{
    public class AuthorListViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IAuthorRepository _repo;

        [Reactive] public AuthorDto? SelectedAuthor { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        [Reactive] public string? ErrorMessage { get; set; }

        public ObservableCollection<AuthorDto> Authors { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public AuthorListViewModel(IAuthorRepository repo, IScreen screen)
        {
            _repo = repo;
            HostScreen = screen;

            var hasSelection = this.WhenAnyValue(x => x.SelectedAuthor)
                .Select(a => a is not null);

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
                Authors.Clear();
                foreach (var a in items) Authors.Add(a);
            }
            finally { IsLoading = false; }
        }

        private async Task DeleteAsync(CancellationToken ct)
        {
            if (SelectedAuthor is null) return;
            try
            {
                ErrorMessage = null;
                await _repo.DeleteAsync(SelectedAuthor.Id, ct);
                Authors.Remove(SelectedAuthor);
                SelectedAuthor = null;
            }
            catch (Exception)
            {
                ErrorMessage = "Нельзя удалить автора пока в библиотеки имеются его книги.";
            }
        }

        private async Task NavigateToAddAsync(CancellationToken ct)
        {
            var vm = new AuthorEditViewModel(_repo, null);
            var window = new AuthorEditView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        private async Task NavigateToEditAsync(CancellationToken ct)
        {
            if (SelectedAuthor is null) return;
            var vm = new AuthorEditViewModel(_repo, SelectedAuthor);
            var window = new AuthorEditView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        public IScreen HostScreen { get; }
        public string UrlPathSegment => "authors";
    }
}
