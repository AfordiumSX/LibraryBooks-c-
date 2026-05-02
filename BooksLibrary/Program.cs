using Avalonia;
using BooksLibrary.Application.Interfaces;
using BooksLibrary.Infrastructure.Data;
using BooksLibrary.Infrastructure.Data.Repositories;
using BooksLibrary.ViewModels.Books;
using BooksLibrary.ViewModels.Loans;
using ReactiveUI.Avalonia;
using System;

namespace BooksLibrary
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
#if DEBUG
                .WithDeveloperTools()
#endif
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
    }
}

