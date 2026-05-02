using BooksLibrary.Application.DTOs;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Domain.Entities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BooksLibrary.ViewModels.Readers
{
    public class ReaderEditViewModel : ReactiveObject
    {
        private static readonly Regex PhoneRegex =
            new(@"^\+?[\d\s\-\(\)]*$", RegexOptions.Compiled);

        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private readonly IReaderRepository _repo;
        private readonly ReaderDto? _existing;

        [Reactive] public string FullName { get; set; } = string.Empty;
        [Reactive] public string Phone { get; set; } = string.Empty;
        [Reactive] public string Email { get; set; } = string.Empty;
        [Reactive] public string? ErrorMessage { get; set; }

        private readonly ObservableAsPropertyHelper<string?> _phoneError;
        public string? PhoneError => _phoneError.Value;

        private readonly ObservableAsPropertyHelper<string?> _emailError;
        public string? EmailError => _emailError.Value;

        public string WindowTitle => _existing is null ? "Добавить читателя" : "Редактировать читателя";

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public Action<bool>? RequestClose { get; set; }

        public ReaderEditViewModel(IReaderRepository repo, ReaderDto? existing)
        {
            _repo = repo;
            _existing = existing;

            if (existing is not null)
            {
                FullName = existing.FullName;
                Phone = existing.Phone;
                Email = existing.Email;
            }

            _phoneError = this.WhenAnyValue(x => x.Phone)
                .Select(ValidatePhone)
                .ToProperty(this, x => x.PhoneError);

            _emailError = this.WhenAnyValue(x => x.Email)
                .Select(ValidateEmail)
                .ToProperty(this, x => x.EmailError);

            var canSave = this.WhenAnyValue(
                x => x.FullName,
                x => x.PhoneError,
                x => x.EmailError,
                (n, pe, ee) =>
                    !string.IsNullOrWhiteSpace(n) && pe is null && ee is null);

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
            CancelCommand = ReactiveCommand.Create(() => RequestClose?.Invoke(false));
        }

        private static string? ValidatePhone(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return PhoneRegex.IsMatch(value)
                ? null
                : "Телефон может содержать только цифры, пробелы, скобки, тире и символ «+».";
        }

        private static string? ValidateEmail(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return EmailRegex.IsMatch(value)
                ? null
                : "E-mail должен быть в формате name@domain.tld (с «@» и точкой в домене).";
        }

        private async Task SaveAsync(CancellationToken ct)
        {
            try
            {
                ErrorMessage = null;
                if (_existing is null)
                {
                    await _repo.AddAsync(new Reader
                    {
                        FullName = FullName,
                        Phone = Phone,
                        Email = Email
                    }, ct);
                }
                else
                {
                    var entity = await _repo.GetByIdAsync(_existing.Id, ct);
                    if (entity is null) return;
                    entity.FullName = FullName;
                    entity.Phone = Phone;
                    entity.Email = Email;
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
