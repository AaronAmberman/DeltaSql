using DeltaSql.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeltaSql.UserControls
{
    public partial class SqlInputUserControl : UserControl
    {
        public SqlInputUserControl()
        {
            InitializeComponent();

            KeyDown += SqlInputUserControl_KeyDown;
        }

        private void SqlInputUserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return) 
            {
                SqlInputViewModel vm = DataContext as SqlInputViewModel;

                if (vm == null) return;

                vm.ConnectIfAble();
            }
        }
    }
}
