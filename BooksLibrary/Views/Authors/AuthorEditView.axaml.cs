using Avalonia.Controls;
using BooksLibrary.ViewModels.Authors;

namespace BooksLibrary.Views.Authors
{
    public partial class AuthorEditView : Window
    {
        public AuthorEditView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) =>
            {
                if (DataContext is AuthorEditViewModel vm)
                    vm.RequestClose = result => Close(result);
            };
        }
    }
}
