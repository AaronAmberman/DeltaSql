using DeltaSql.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeltaSql.UserControls
{
    public partial class SqlConnectionInputUserControl : UserControl
    {
        public SqlConnectionInputUserControl()
        {
            InitializeComponent();

            KeyDown += SqlConnectionInputUserControl_KeyDown;
        }

        private void SqlConnectionInputUserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return) 
            {
                SqlInputViewModel vm = DataContext as SqlInputViewModel;
                vm?.ConnectIfAble();
            }
        }
    }
}
