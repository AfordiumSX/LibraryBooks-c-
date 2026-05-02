using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Infrastructure.Data;
using BooksLibrary.Infrastructure.Data.Repositories;
using BooksLibrary.ViewModels;
using BooksLibrary.ViewModels.Authors;
using BooksLibrary.ViewModels.Books;
using BooksLibrary.ViewModels.Genres;
using BooksLibrary.ViewModels.Loans;
using BooksLibrary.ViewModels.Readers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BooksLibrary
{
    public partial class App : Avalonia.Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();

            services.AddDbContextFactory<AppDbContext>(opt =>
                opt.UseNpgsql("Host=АДРЕС;Port=5432;Database=НАЗВАНИЕ_БАЗЫ_ДАННЫХ;Username=ИМЯ_ПОЛЬЗОВАТЕЛЯ;Password=ПАРОЛЬ"));

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IReaderRepository, ReaderRepository>();
            services.AddScoped<ILoanRepository, LoanRepository>();

            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<BookListViewModel>();
            services.AddTransient<AuthorListViewModel>();
            services.AddTransient<GenreListViewModel>();
            services.AddTransient<ReaderListViewModel>();
            services.AddTransient<LoanListViewModel>();

            Services = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainVm = Services.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow = new Views.MainWindow
                {
                    DataContext = mainVm
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
