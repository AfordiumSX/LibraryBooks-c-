using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Domain.Entities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.ViewModels.Genres
{
    public class GenreEditViewModel : ReactiveObject
    {
        private readonly IGenreRepository _repo;
        private readonly GenreDto? _existing;

        [Reactive] public string Name { get; set; } = string.Empty;
        [Reactive] public string? ErrorMessage { get; set; }

        public string WindowTitle => _existing is null ? "Добавить жанр" : "Редактировать жанр";

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public Action<bool>? RequestClose { get; set; }

        public GenreEditViewModel(IGenreRepository repo, GenreDto? existing)
        {
            _repo = repo;
            _existing = existing;

            if (existing is not null)
            {
                Name = existing.Name;
            }

            var canSave = this.WhenAnyValue(x => x.Name)
                .Select(n => !string.IsNullOrWhiteSpace(n));

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
            CancelCommand = ReactiveCommand.Create(() => RequestClose?.Invoke(false));
        }

        private async Task SaveAsync(CancellationToken ct)
        {
            try
            {
                ErrorMessage = null;
                if (_existing is null)
                {
                    await _repo.AddAsync(new Genre { Name = Name }, ct);
                }
                else
                {
                    var entity = await _repo.GetByIdAsync(_existing.Id, ct);
                    if (entity is null) return;
                    entity.Name = Name;
                    await _repo.UpdateAsync(entity, ct);
                }
                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
