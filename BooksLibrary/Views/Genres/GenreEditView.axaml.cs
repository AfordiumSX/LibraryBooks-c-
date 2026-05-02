using Avalonia.Controls;
using BooksLibrary.ViewModels.Genres;

namespace BooksLibrary.Views.Genres
{
    public partial class GenreEditView : Window
    {
        public GenreEditView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) =>
            {
                if (DataContext is GenreEditViewModel vm)
                    vm.RequestClose = result => Close(result);
            };
        }
    }
}
