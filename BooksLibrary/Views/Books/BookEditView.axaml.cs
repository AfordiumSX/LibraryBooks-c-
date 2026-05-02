using Avalonia.Controls;
using BooksLibrary.ViewModels.Books;

namespace BooksLibrary.Views.Books
{
    public partial class BookEditView : Window
    {
        public BookEditView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) =>
            {
                if (DataContext is BookEditViewModel vm)
                    vm.RequestClose = result => Close(result);
            };
        }
    }
}
