using Avalonia.Styling;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.ViewModels.Authors;
using BooksLibrary.ViewModels.Books;
using BooksLibrary.ViewModels.Genres;
using BooksLibrary.ViewModels.Loans;
using BooksLibrary.ViewModels.Readers;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace BooksLibrary.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router { get; } = new RoutingState();

        private readonly ObservableAsPropertyHelper<IRoutableViewModel?> _currentViewModel;
        public IRoutableViewModel? CurrentViewModel => _currentViewModel.Value;

        [Reactive] public string ThemeButtonText { get; set; } = "Светлая тема";

        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToBooksCommand { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToLoansCommand { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToAuthorsCommand { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToGenresCommand { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToReadersCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }

        public MainWindowViewModel()
        {
            _currentViewModel = Router.CurrentViewModel
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CurrentViewModel);

            NavigateToBooksCommand = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(
                    new BookListViewModel(
                        App.Services.GetRequiredService<IBookRepository>(),
                        this)));

            NavigateToLoansCommand = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(
                    new LoanListViewModel(
                        App.Services.GetRequiredService<ILoanRepository>(),
                        this)));

            NavigateToAuthorsCommand = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(
                    new AuthorListViewModel(
                        App.Services.GetRequiredService<IAuthorRepository>(),
                        this)));

            NavigateToGenresCommand = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(
                    new GenreListViewModel(
                        App.Services.GetRequiredService<IGenreRepository>(),
                        this)));

            NavigateToReadersCommand = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(
                    new ReaderListViewModel(
                        App.Services.GetRequiredService<IReaderRepository>(),
                        this)));

            ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);

            UpdateThemeButtonText();
            NavigateToBooksCommand.Execute().Subscribe();
        }

        private void ToggleTheme()
        {
            var app = Avalonia.Application.Current;
            if (app is null) return;

            var current = app.ActualThemeVariant;
            app.RequestedThemeVariant = current == ThemeVariant.Dark
                ? ThemeVariant.Light
                : ThemeVariant.Dark;

            UpdateThemeButtonText();
        }

        private void UpdateThemeButtonText()
        {
            var app = Avalonia.Application.Current;
            if (app is null) return;

            ThemeButtonText = app.ActualThemeVariant == ThemeVariant.Dark
                ? "Светлая тема"
                : "Темная тема";
        }
    }
}
