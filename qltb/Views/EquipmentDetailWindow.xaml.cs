using System.Linq;
using System.Windows;
using qltb.Models;

namespace qltb.Views
{
    public partial class EquipmentDetailWindow : Window
    {
        public EquipmentDetailWindow(EquipmentItem equipment)
        {
            InitializeComponent();
            LoadEquipment(equipment);
        }

        private void LoadEquipment(EquipmentItem equipment)
        {
            if (equipment == null) return;

            TxtEquipmentName.Text = equipment.Name;
            TxtQRCode.Text = equipment.QRCode;
            TxtCategory.Text = equipment.Category.ToString();
            TxtQuantity.Text = equipment.Quantity.ToString();
            TxtStatus.Text = equipment.Status ?? "Chưa xác định";
            TxtLocation.Text = equipment.Location ?? "Chưa xác định";
            TxtCreatedAt.Text = equipment.CreatedAt.ToString("dd/MM/yyyy HH:mm");

            // Thông số kỹ thuật
            if (equipment.Spec != null)
            {
                TxtCaliber.Text = equipment.Spec.Caliber ?? "N/A";
                TxtWeight.Text = equipment.Spec.Weight ?? "N/A";
                TxtRange.Text = equipment.Spec.Range ?? "N/A";
                TxtMaterial.Text = equipment.Spec.Material ?? "N/A";
            }

            // Tính năng kỹ chiến thuật
            if (equipment.TacticalFeatures != null && equipment.TacticalFeatures.Any())
            {
                LstFeatures.ItemsSource = equipment.TacticalFeatures;
                TxtNoFeatures.Visibility = Visibility.Collapsed;
            }
            else
            {
                LstFeatures.Visibility = Visibility.Collapsed;
                TxtNoFeatures.Visibility = Visibility.Visible;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}