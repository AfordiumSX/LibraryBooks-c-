using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Domain.Entities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.ViewModels.Books
{
    public class BookEditViewModel : ReactiveObject
    {
        private readonly IBookRepository _bookRepo;
        private readonly IAuthorRepository _authorRepo;
        private readonly IGenreRepository _genreRepo;
        private readonly BookDto? _existing;

        [Reactive] public string Title { get; set; } = string.Empty;
        [Reactive] public string ISBN { get; set; } = string.Empty;
        [Reactive] public int Year { get; set; } = 2024;
        [Reactive] public int Quantity { get; set; } = 1;
        [Reactive] public AuthorDto? SelectedAuthor { get; set; }
        [Reactive] public GenreDto? SelectedGenre { get; set; }
        [Reactive] public string? ErrorMessage { get; set; }

        public string WindowTitle => _existing is null ? "Добавить книгу" : "Редактировать книгу";

        public ObservableCollection<AuthorDto> Authors { get; } = new();
        public ObservableCollection<GenreDto> Genres { get; } = new();

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        private ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public Action<bool>? RequestClose { get; set; }

        public BookEditViewModel(
            IBookRepository bookRepo,
            IAuthorRepository authorRepo,
            IGenreRepository genreRepo,
            BookDto? existing)
        {
            _bookRepo = bookRepo;
            _authorRepo = authorRepo;
            _genreRepo = genreRepo;
            _existing = existing;

            if (existing is not null)
            {
                Title = existing.Title;
                ISBN = existing.ISBN;
                Year = existing.Year;
                Quantity = existing.Quantity;
            }

            var canSave = this.WhenAnyValue(
                x => x.Title,
                x => x.SelectedAuthor,
                x => x.SelectedGenre,
                (t, a, g) => !string.IsNullOrWhiteSpace(t) && a != null && g != null);

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
            CancelCommand = ReactiveCommand.Create(() => RequestClose?.Invoke(false));
            LoadCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
            LoadCommand.Execute().Subscribe();
        }

        private async Task LoadDataAsync(CancellationToken ct)
        {
            var authors = await _authorRepo.GetAllAsync(ct);
            var genres = await _genreRepo.GetAllAsync(ct);

            Authors.Clear();
            foreach (var a in authors) Authors.Add(a);

            Genres.Clear();
            foreach (var g in genres) Genres.Add(g);

            if (_existing is not null)
            {
                SelectedAuthor = Authors.FirstOrDefault(a => a.Id == _existing.AuthorId);
                SelectedGenre = Genres.FirstOrDefault(g => g.Id == _existing.GenreId);
            }
        }

        private async Task SaveAsync(CancellationToken ct)
        {
            if (SelectedAuthor is null || SelectedGenre is null) return;

            try
            {
                ErrorMessage = null;
                if (_existing is null)
                {
                    await _bookRepo.AddAsync(new Book
                    {
                        Title = Title,
                        ISBN = ISBN,
                        Year = Year,
                        Quantity = Quantity,
                        AuthorId = SelectedAuthor.Id,
                        GenreId = SelectedGenre.Id
                    }, ct);
                }
                else
                {
                    var book = await _bookRepo.GetByIdAsync(_existing.Id, ct);
                    if (book is null) return;

                    book.Title = Title;
                    book.ISBN = ISBN;
                    book.Year = Year;
                    book.Quantity = Quantity;
                    book.AuthorId = SelectedAuthor.Id;
                    book.GenreId = SelectedGenre.Id;

                    await _bookRepo.UpdateAsync(book, ct);
                }

                RequestClose?.Invoke(true);
            }
            catch (Exception)
            {
                ErrorMessage = "Для добавления книги укажите ISBN.";
            }
        }
    }
}
