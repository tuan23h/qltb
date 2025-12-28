using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using qltb.Models;

namespace qltb.Views
{
    public partial class EquipmentEditDialog : Window
    {
        public EquipmentItem Equipment { get; private set; }
        private List<TacticalFeature> _features = new List<TacticalFeature>();

        public EquipmentEditDialog(EquipmentItem existingItem = null)
        {
            InitializeComponent();

            // Load categories
            CmbCategory.ItemsSource = Enum.GetValues(typeof(EquipmentCategory));
            CmbCategory.SelectedIndex = 0;

            // Default status
            CmbStatus.SelectedIndex = 0;

            if (existingItem != null)
            {
                TxtTitle.Text = "CẬP NHẬT TRANG BỊ";
                LoadEquipment(existingItem);
            }
        }

        private void LoadEquipment(EquipmentItem item)
        {
            TxtQRCode.Text = item.QRCode;
            TxtName.Text = item.Name;
            CmbCategory.SelectedItem = item.Category;
            TxtQuantity.Text = item.Quantity.ToString();
            CmbStatus.Text = item.Status;
            TxtLocation.Text = item.Location;

            if (item.Spec != null)
            {
                TxtCaliber.Text = item.Spec.Caliber;
                TxtWeight.Text = item.Spec.Weight;
                TxtRange.Text = item.Spec.Range;
                TxtMaterial.Text = item.Spec.Material;
            }

            if (item.TacticalFeatures != null)
            {
                _features = item.TacticalFeatures.ToList();
                LstFeatures.ItemsSource = null;
                LstFeatures.ItemsSource = _features;
            }

            Equipment = item;
        }

        private void BtnAddFeature_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog("Thêm tính năng kỹ - chiến thuật", "Nhập mô tả:");
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
            {
                _features.Add(new TacticalFeature { Description = dialog.ResponseText });
                LstFeatures.ItemsSource = null;
                LstFeatures.ItemsSource = _features;
            }
        }

        private void BtnRemoveFeature_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var feature = button?.Tag as TacticalFeature;
            if (feature != null)
            {
                _features.Remove(feature);
                LstFeatures.ItemsSource = null;
                LstFeatures.ItemsSource = _features;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(TxtQRCode.Text))
                {
                    MessageBox.Show("Vui lòng nhập QR Code", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtQRCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên trang bị", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtName.Focus();
                    return;
                }

                if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên không âm", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtQuantity.Focus();
                    return;
                }

                // Create or update equipment
                if (Equipment == null)
                {
                    Equipment = new EquipmentItem();
                }

                Equipment.QRCode = TxtQRCode.Text.Trim();
                Equipment.Name = TxtName.Text.Trim();
                Equipment.Category = (EquipmentCategory)CmbCategory.SelectedItem;
                Equipment.Quantity = quantity;
                Equipment.Status = (CmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Sẵn sàng";
                Equipment.Location = TxtLocation.Text.Trim();

                Equipment.Spec = new EquipmentSpec
                {
                    Caliber = TxtCaliber.Text.Trim(),
                    Weight = TxtWeight.Text.Trim(),
                    Range = TxtRange.Text.Trim(),
                    Material = TxtMaterial.Text.Trim()
                };

                Equipment.TacticalFeatures = _features.ToList();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    // Helper Dialog for Input
    public class InputDialog : Window
    {
        private TextBox _txtInput;

        public string ResponseText => _txtInput.Text;

        public InputDialog(string title, string prompt)
        {
            Title = title;
            Width = 400;
            Height = 180;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock
            {
                Text = prompt,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            _txtInput = new TextBox
            {
                Height = 35,
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 15)
            };
            stack.Children.Add(_txtInput);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

            var btnOk = new Button
            {
                Content = "OK",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0),
                Background = System.Windows.Media.Brushes.DodgerBlue,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0)
            };
            btnOk.Click += (s, e) => { DialogResult = true; Close(); };

            var btnCancel = new Button
            {
                Content = "Hủy",
                Width = 80,
                Height = 30,
                Background = System.Windows.Media.Brushes.Gray,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0)
            };
            btnCancel.Click += (s, e) => { DialogResult = false; Close(); };

            btnPanel.Children.Add(btnOk);
            btnPanel.Children.Add(btnCancel);
            stack.Children.Add(btnPanel);

            Content = stack;
            _txtInput.Focus();
        }
    }
}