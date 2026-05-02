using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Views;
using BooksLibrary.Views.Books;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.ViewModels.Books
{
    public class BookListViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IBookRepository _bookRepo;

        [Reactive] public string SearchText { get; set; } = string.Empty;
        [Reactive] public BookDto? SelectedBook { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        [Reactive] public string? ErrorMessage { get; set; }

        public ObservableCollection<BookDto> Books { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public BookListViewModel(IBookRepository bookRepo, IScreen screen)
        {
            _bookRepo = bookRepo;
            HostScreen = screen;

            var hasSelection = this.WhenAnyValue(x => x.SelectedBook)
                .Select(b => b is not null);

            LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync);
            AddCommand = ReactiveCommand.CreateFromTask(NavigateToAddAsync);
            EditCommand = ReactiveCommand.CreateFromTask(NavigateToEditAsync, hasSelection);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync, hasSelection);

            this.WhenAnyValue(x => x.SearchText)
                .Skip(1)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .SelectMany(async t =>
                {
                    try
                    {
                        return string.IsNullOrWhiteSpace(t)
                            ? await _bookRepo.GetAllAsync()
                            : await _bookRepo.SearchAsync(t);
                    }
                    catch
                    {
                        return new System.Collections.Generic.List<BookDto>();
                    }
                })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(results =>
                {
                    Books.Clear();
                    foreach (var b in results) Books.Add(b);
                });

            LoadCommand.Execute().Subscribe();
        }

        private async Task LoadAsync(CancellationToken ct)
        {
            IsLoading = true;
            try
            {
                ErrorMessage = null;
                var items = await _bookRepo.GetAllAsync(ct);
                Books.Clear();
                foreach (var b in items) Books.Add(b);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка загрузки: " + ex.Message;
            }
            finally { IsLoading = false; }
        }

        private async Task DeleteAsync(CancellationToken ct)
        {
            if (SelectedBook is null) return;
            try
            {
                ErrorMessage = null;
                await _bookRepo.DeleteAsync(SelectedBook.Id, ct);
                Books.Remove(SelectedBook);
                SelectedBook = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка удаления: " + ex.Message;
            }
        }

        private async Task NavigateToAddAsync(CancellationToken ct)
        {
            var vm = new BookEditViewModel(
                App.Services.GetRequiredService<IBookRepository>(),
                App.Services.GetRequiredService<IAuthorRepository>(),
                App.Services.GetRequiredService<IGenreRepository>(),
                null);
            var window = new BookEditView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        private async Task NavigateToEditAsync(CancellationToken ct)
        {
            if (SelectedBook is null) return;
            var vm = new BookEditViewModel(
                App.Services.GetRequiredService<IBookRepository>(),
                App.Services.GetRequiredService<IAuthorRepository>(),
                App.Services.GetRequiredService<IGenreRepository>(),
                SelectedBook);
            var window = new BookEditView { DataContext = vm };
            await DialogService.ShowDialogAsync(window);
            await LoadAsync(ct);
        }

        public IScreen HostScreen { get; }
        public string UrlPathSegment => "books";
    }
}
