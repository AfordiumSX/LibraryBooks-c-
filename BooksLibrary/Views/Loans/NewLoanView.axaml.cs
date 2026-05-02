using Avalonia.Controls;
using BooksLibrary.ViewModels.Loans;

namespace BooksLibrary.Views.Loans
{
    public partial class NewLoanView : Window
    {
        public NewLoanView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) =>
            {
                if (DataContext is NewLoanViewModel vm)
                    vm.RequestClose = result => Close(result);
            };
        }
    }
}
