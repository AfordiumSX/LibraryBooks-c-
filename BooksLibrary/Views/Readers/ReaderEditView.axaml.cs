using Avalonia.Controls;
using BooksLibrary.ViewModels.Readers;

namespace BooksLibrary.Views.Readers
{
    public partial class ReaderEditView : Window
    {
        public ReaderEditView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) =>
            {
                if (DataContext is ReaderEditViewModel vm)
                    vm.RequestClose = result => Close(result);
            };
        }
    }
}
