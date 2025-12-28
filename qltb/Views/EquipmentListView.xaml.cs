using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using qltb.ViewModels;

namespace qltb.Views
{
    public partial class EquipmentListView : UserControl
    {
        public EquipmentListView()
        {
            InitializeComponent();
            DataContext = new EquipmentListViewModel();

            // Thêm converter cho visibility
            Resources.Add("BoolToVisibilityConverter", new BooleanToVisibilityConverter());
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as EquipmentListViewModel;
            if (vm?.SelectedItem != null)
            {
                vm.ViewDetailCommand.Execute(null);
            }
        }
    }
}