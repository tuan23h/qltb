using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using qltb.Models;
using qltb.Services;

namespace qltb.Views
{
    public partial class StatisticsView : UserControl
    {
        private EquipmentService _service;

        public StatisticsView()
        {
            InitializeComponent();
            _service = new EquipmentService();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var stats = _service.GetStatistics();

            TxtTotalItems.Text = stats["TotalItems"].ToString();
            TxtWeapons.Text = stats["Weapons"].ToString();
            TxtAmmunition.Text = stats["Ammunition"].ToString();
            TxtGear.Text = stats["Gear"].ToString();
            TxtAvailable.Text = stats["Available"].ToString();
            TxtInUse.Text = stats["InUse"].ToString();
            TxtMaintenance.Text = stats["Maintenance"].ToString();
        }

        private void TotalCard_Click(object sender, MouseButtonEventArgs e)
        {
            var items = _service.GetAll();
            DetailDataGrid.ItemsSource = items;
            TxtDetailTitle.Text = $"Tất cả trang bị ({items.Count} mục)";
        }

        private void WeaponsCard_Click(object sender, MouseButtonEventArgs e)
        {
            var items = _service.GetByCategory(EquipmentCategory.Weapon);
            DetailDataGrid.ItemsSource = items;
            TxtDetailTitle.Text = $"Vũ khí ({items.Count} mục)";
        }

        private void AmmunitionCard_Click(object sender, MouseButtonEventArgs e)
        {
            var items = _service.GetByCategory(EquipmentCategory.Ammunition);
            DetailDataGrid.ItemsSource = items;
            TxtDetailTitle.Text = $"Đạn dược ({items.Count} mục)";
        }

        private void GearCard_Click(object sender, MouseButtonEventArgs e)
        {
            var items = _service.GetByCategory(EquipmentCategory.Gear);
            DetailDataGrid.ItemsSource = items;
            TxtDetailTitle.Text = $"Trang bị khác ({items.Count} mục)";
        }

        private void AvailableCard_Click(object sender, MouseButtonEventArgs e)
        {
            var items = _service.GetByStatus("Sẵn sàng");
            DetailDataGrid.ItemsSource = items;
            TxtDetailTitle.Text = $"Trang bị sẵn sàng ({items.Count} mục)";
        }

        private void InUseCard_Click(object sender, MouseButtonEventArgs e)
        {
            var items = _service.GetByStatus("Đang sử dụng");
            DetailDataGrid.ItemsSource = items;
            TxtDetailTitle.Text = $"Trang bị đang sử dụng ({items.Count} mục)";
        }

        private void MaintenanceCard_Click(object sender, MouseButtonEventArgs e)
        {
            var items = _service.GetByStatus("Bảo trì");
            DetailDataGrid.ItemsSource = items;
            TxtDetailTitle.Text = $"Trang bị bảo trì ({items.Count} mục)";
        }
    }
}