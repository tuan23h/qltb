using System.Windows.Controls;
using qltb.Models;
using qltb.ViewModels;

namespace qltb.Views
{
    public partial class EquipmentListView : UserControl
    {
        public EquipmentListView()
        {
            InitializeComponent();
            DataContext = new EquipmentListViewModel();
        }

        private void OnRowDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (((DataGrid)sender).SelectedItem is EquipmentItem item)
            {
                var detailView = new EquipmentDetailView
                {
                    DataContext = new EquipmentDetailViewModel(item)
                };

                ((MainViewModel)System.Windows.Application.Current.MainWindow.DataContext)
                    .CurrentView = detailView;
            }
        }
    }
}
