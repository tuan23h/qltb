using System.Windows;
using qltb.Data;
using qltb.Views;

namespace qltb
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries_V2.Init();
            base.OnStartup(e);

            // Khởi tạo database
            using (var db = new AppDbContext())
            {
                DbInitializer.Initialize(db);
            }

            // Hiển thị màn hình đăng nhập
            var loginView = new LoginView();
            loginView.Show();

            // Đặt làm main window để app không tắt khi close window khác
            MainWindow = loginView;
        }
    }
}