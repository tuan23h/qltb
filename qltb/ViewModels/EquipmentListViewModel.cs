using OfficeOpenXml;
using qltb.Models;
using qltb.Services;
using qltb.Utils;
using qltb.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace qltb.ViewModels
{
    public class EquipmentListViewModel : BaseViewModel
    {
        private readonly EquipmentService _service;
        private string _searchKeyword;
        private EquipmentItem _selectedItem;
        private bool _isAdmin;

        public ObservableCollection<EquipmentItemWrapper> Equipments { get; set; }

        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchCommand.Execute(null);
            }
        }

        public EquipmentItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ExportExcelCommand { get; }

        public EquipmentListViewModel()
        {
            _service = new EquipmentService();
            Equipments = new ObservableCollection<EquipmentItemWrapper>();

            // Kiểm tra quyền admin
            IsAdmin = AuthService.IsAdmin;

            LoadDataCommand = new RelayCommand(LoadData);
            SearchCommand = new RelayCommand(Search);
            AddCommand = new RelayCommand(Add, () => IsAdmin);
            EditCommand = new RelayCommand(Edit, () => IsAdmin && SelectedItem != null);
            DeleteCommand = new RelayCommand(Delete, () => IsAdmin && SelectedItem != null);
            ViewDetailCommand = new RelayCommand(ViewDetail, () => SelectedItem != null);
            RefreshCommand = new RelayCommand(LoadData);
            ExportExcelCommand = new RelayCommand(ExportExcel);

            LoadData();
        }

        private void LoadData()
        {
            Equipments.Clear();
            var items = _service.GetAll();
            foreach (var item in items)
            {
                Equipments.Add(new EquipmentItemWrapper(item));
            }
        }

        private void Search()
        {
            Equipments.Clear();
            var items = _service.Search(SearchKeyword);
            foreach (var item in items)
            {
                Equipments.Add(new EquipmentItemWrapper(item));
            }
        }

        private void Add()
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền thực hiện thao tác này!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EquipmentEditDialog();
            if (dialog.ShowDialog() == true && dialog.Equipment != null)
            {
                try
                {
                    _service.Add(dialog.Equipment);
                    LoadData();
                    MessageBox.Show("Thêm trang bị thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Edit()
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền thực hiện thao tác này!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedItem == null) return;

            var dialog = new EquipmentEditDialog(SelectedItem);
            if (dialog.ShowDialog() == true && dialog.Equipment != null)
            {
                try
                {
                    _service.Update(dialog.Equipment);
                    LoadData();
                    MessageBox.Show("Cập nhật thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Delete()
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền thực hiện thao tác này!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedItem == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa '{SelectedItem.Name}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _service.Delete(SelectedItem.Id);
                    LoadData();
                    MessageBox.Show("Xóa thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewDetail()
        {
            if (SelectedItem == null) return;

            var detailWindow = new EquipmentDetailWindow(SelectedItem);
            detailWindow.ShowDialog();
        }

        private void ExportExcel()
        {
            var selectedItems = Equipments.Where(x => x.IsSelected).Select(x => x.Item).ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một trang bị để xuất Excel!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = $"DanhSachTrangBi_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Danh sách trang bị");

                        // Header
                        worksheet.Cells[1, 1].Value = "QR Code";
                        worksheet.Cells[1, 2].Value = "Tên trang bị";
                        worksheet.Cells[1, 3].Value = "Loại";
                        worksheet.Cells[1, 4].Value = "Số lượng";
                        worksheet.Cells[1, 5].Value = "Trạng thái";
                        worksheet.Cells[1, 6].Value = "Vị trí";
                        worksheet.Cells[1, 7].Value = "Cỡ đạn";
                        worksheet.Cells[1, 8].Value = "Khối lượng";
                        worksheet.Cells[1, 9].Value = "Tầm bắn";
                        worksheet.Cells[1, 10].Value = "Vật liệu";
                        worksheet.Cells[1, 11].Value = "Ngày tạo";

                        // Style header
                        using (var range = worksheet.Cells[1, 1, 1, 11])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        }

                        // Data
                        int row = 2;
                        foreach (var item in selectedItems)
                        {
                            worksheet.Cells[row, 1].Value = item.QRCode;
                            worksheet.Cells[row, 2].Value = item.Name;
                            worksheet.Cells[row, 3].Value = item.Category.ToString();
                            worksheet.Cells[row, 4].Value = item.Quantity;
                            worksheet.Cells[row, 5].Value = item.Status;
                            worksheet.Cells[row, 6].Value = item.Location;
                            worksheet.Cells[row, 7].Value = item.Spec?.Caliber ?? "";
                            worksheet.Cells[row, 8].Value = item.Spec?.Weight ?? "";
                            worksheet.Cells[row, 9].Value = item.Spec?.Range ?? "";
                            worksheet.Cells[row, 10].Value = item.Spec?.Material ?? "";
                            worksheet.Cells[row, 11].Value = item.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                            row++;
                        }

                        // Auto-fit columns
                        worksheet.Cells.AutoFitColumns();

                        // Save
                        var file = new FileInfo(saveDialog.FileName);
                        package.SaveAs(file);
                    }

                    MessageBox.Show($"Xuất Excel thành công!\nĐã xuất {selectedItems.Count} trang bị.",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở file
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Wrapper class để thêm checkbox
    public class EquipmentItemWrapper : BaseViewModel
    {
        private bool _isSelected;

        public EquipmentItem Item { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        // Proxy properties để binding
        public int Id => Item.Id;
        public string QRCode => Item.QRCode;
        public string Name => Item.Name;
        public EquipmentCategory Category => Item.Category;
        public int Quantity => Item.Quantity;
        public string Status => Item.Status;
        public string Location => Item.Location;
        public DateTime CreatedAt => Item.CreatedAt;

        public EquipmentItemWrapper(EquipmentItem item)
        {
            Item = item;
            IsSelected = false;
        }
    }
}