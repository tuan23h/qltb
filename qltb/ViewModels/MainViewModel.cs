using System.Windows;
using System.Windows.Input;
using qltb.Services;
using qltb.Utils;
using qltb.Views;

namespace qltb.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentView;
        private string _currentUserName;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public string CurrentUserName
        {
            get => _currentUserName;
            set
            {
                _currentUserName = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToListCommand { get; }
        public ICommand NavigateToScanCommand { get; }
        public ICommand NavigateToStatisticsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            // Khởi tạo commands
            NavigateToListCommand = new RelayCommand(NavigateToList);
            NavigateToScanCommand = new RelayCommand(NavigateToScan);
            NavigateToStatisticsCommand = new RelayCommand(NavigateToStatistics);
            LogoutCommand = new RelayCommand(Logout);

            // Hiển thị thông tin user
            if (AuthService.CurrentUser != null)
            {
                CurrentUserName = $"{AuthService.CurrentUser.FullName} ({AuthService.CurrentUser.Role})";
            }
            else
            {
                CurrentUserName = "Guest";
            }

            // Mặc định hiển thị danh sách
            NavigateToList();
        }

        private void NavigateToList()
        {
            CurrentView = new EquipmentListView();
        }

        private void NavigateToScan()
        {
            CurrentView = new ScanView();
        }

        private void NavigateToStatistics()
        {
            CurrentView = new StatisticsView();
        }

        private void Logout()
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var authService = new AuthService();
                authService.Logout();

                // Tìm tất cả các Window đang mở
                var windows = Application.Current.Windows;
                Window mainWindow = null;

                foreach (Window window in windows)
                {
                    if (window is MainWindow)
                    {
                        mainWindow = window;
                        break;
                    }
                }

                // Mở LoginView
                var loginView = new LoginView();
                Application.Current.MainWindow = loginView;
                loginView.Show();

                // Đóng MainWindow
                if (mainWindow != null)
                {
                    mainWindow.Close();
                }
            }
        }
    }
}