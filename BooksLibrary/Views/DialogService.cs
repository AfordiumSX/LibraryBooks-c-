using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace BooksLibrary.Views
{
    public static class DialogService
    {
        public static Window? GetMainWindow()
        {
            return (Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                ?.MainWindow;
        }

        public static Task<bool?> ShowDialogAsync(Window dialog)
        {
            var owner = GetMainWindow();
            if (owner is null)
            {
                dialog.Show();
                return Task.FromResult<bool?>(null);
            }
            return dialog.ShowDialog<bool?>(owner);
        }
    }
}
