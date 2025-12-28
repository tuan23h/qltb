using System.Windows;
using qltb.Data;
using qltb.Views;

namespace qltb
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Khởi tạo database
            try
            {
                using (var db = new AppDbContext())
                {
                    // Tạo database nếu chưa tồn tại
                    db.Database.EnsureCreated();

                    // Khởi tạo dữ liệu mẫu
                    DbInitializer.Initialize(db);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khởi tạo database:\n{ex.Message}\n\n" +
                    "Vui lòng đảm bảo SQL Server LocalDB đã được cài đặt.\n" +
                    "Download tại: https://aka.ms/SSMSFullSetup",
                    "Lỗi Database",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // Hiển thị màn hình đăng nhập
            var loginView = new LoginView();
            loginView.Show();

            // Đặt làm main window để app không tắt khi close window khác
            MainWindow = loginView;
        }
    }
}