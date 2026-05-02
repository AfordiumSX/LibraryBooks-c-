using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Views;
using BooksLibrary.Views.Genres;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.ViewModels.Genres
{
    public class GenreListViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IGenreRepository _repo;

        [Reactive] public GenreDto? SelectedGenre { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        [Reactive] public string? ErrorMessage { get; set; }

        public ObservableCollection<GenreDto> Genres { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public GenreListViewModel(IGenreRepository repo, IScreen screen)
        {
            _repo = repo;
            HostScreen = screen;

            var hasSelection = this.WhenAnyValue(x => x.SelectedGenre)
                .Select(g => g is not null);

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
                Genres.Clear();
                foreach (var g in items) Genres.Add(g);
            }
            finally { IsLoading = false; }
        }

        private async Task DeleteAsync(CancellationToken ct)
        {
            if (SelectedGenre is null) return;
            try
            {
                ErrorMessage = null;
                await _repo.DeleteAsync(SelectedGenre.Id, ct);
                Genres.Remove(SelectedGenre);
                SelectedGenre = null;
            }
            catch (Exception)
            {
                ErrorMessage = "Нельзя удалить жанр пока в бибилиотеки имеются книги с этим жанром.";
            }
        }

        private async Task NavigateToAddAsync(CancellationToken ct)
        {
            var vm = new GenreEditViewModel(_repo, null);
            var window = new GenreEditView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        private async Task NavigateToEditAsync(CancellationToken ct)
        {
            if (SelectedGenre is null) return;
            var vm = new GenreEditViewModel(_repo, SelectedGenre);
            var window = new GenreEditView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        public IScreen HostScreen { get; }
        public string UrlPathSegment => "genres";
    }
}
