using System.Windows.Controls;
using qltb.Services;

namespace qltb.Views
{
    public partial class StatisticsView : UserControl
    {
        public StatisticsView()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            using (var service = new EquipmentService())
            {
                var stats = service.GetStatistics();

                TxtTotalItems.Text = stats["TotalItems"].ToString();
                TxtWeapons.Text = stats["Weapons"].ToString();
                TxtAmmunition.Text = stats["Ammunition"].ToString();
                TxtGear.Text = stats["Gear"].ToString();
                TxtAvailable.Text = stats["Available"].ToString();
                TxtInUse.Text = stats["InUse"].ToString();
                TxtMaintenance.Text = stats["Maintenance"].ToString();
            }
        }
    }
}