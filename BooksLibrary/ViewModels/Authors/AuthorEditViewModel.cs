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

namespace BooksLibrary.ViewModels.Authors
{
    public class AuthorEditViewModel : ReactiveObject
    {
        private readonly IAuthorRepository _repo;
        private readonly AuthorDto? _existing;

        [Reactive] public string FirstName { get; set; } = string.Empty;
        [Reactive] public string LastName { get; set; } = string.Empty;
        [Reactive] public string? ErrorMessage { get; set; }

        public string WindowTitle => _existing is null ? "Добавить автора" : "Редактировать автора";

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public Action<bool>? RequestClose { get; set; }

        public AuthorEditViewModel(IAuthorRepository repo, AuthorDto? existing)
        {
            _repo = repo;
            _existing = existing;

            if (existing is not null)
            {
                FirstName = existing.FirstName;
                LastName = existing.LastName;
            }

            var canSave = this.WhenAnyValue(
                x => x.FirstName,
                x => x.LastName,
                (f, l) => !string.IsNullOrWhiteSpace(f) && !string.IsNullOrWhiteSpace(l));

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
                    await _repo.AddAsync(new Author { FirstName = FirstName, LastName = LastName }, ct);
                }
                else
                {
                    var entity = await _repo.GetByIdAsync(_existing.Id, ct);
                    if (entity is null) return;
                    entity.FirstName = FirstName;
                    entity.LastName = LastName;
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
