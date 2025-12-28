using qltb.Models;
using qltb.Services;
using qltb.Utils;
using qltb.Views;
using System.Collections.ObjectModel;
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

        public ObservableCollection<EquipmentItem> Equipments { get; set; }

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

        public ICommand LoadDataCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand RefreshCommand { get; }

        public EquipmentListViewModel()
        {
            _service = new EquipmentService();
            Equipments = new ObservableCollection<EquipmentItem>();

            LoadDataCommand = new RelayCommand(LoadData);
            SearchCommand = new RelayCommand(Search);
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, () => SelectedItem != null);
            DeleteCommand = new RelayCommand(Delete, () => SelectedItem != null);
            ViewDetailCommand = new RelayCommand(ViewDetail, () => SelectedItem != null);
            RefreshCommand = new RelayCommand(LoadData);

            LoadData();
        }

        private void LoadData()
        {
            Equipments.Clear();
            var items = _service.GetAll();
            foreach (var item in items)
            {
                Equipments.Add(item);
            }
        }

        private void Search()
        {
            Equipments.Clear();
            var items = _service.Search(SearchKeyword);
            foreach (var item in items)
            {
                Equipments.Add(item);
            }
        }

        private void Add()
        {
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
    }
}