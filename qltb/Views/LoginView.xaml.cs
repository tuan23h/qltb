using System;
using System.Windows;
using System.Windows.Input;
using qltb.Services;

namespace qltb.Views
{
    public partial class LoginView : Window
    {
        private readonly AuthService _authService;

        public LoginView()
        {
            InitializeComponent();
            _authService = new AuthService();
            TxtUsername.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private void PerformLogin()
        {
            TxtError.Visibility = Visibility.Collapsed;

            var username = TxtUsername.Text.Trim();
            var password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Vui lòng nhập tên đăng nhập");
                TxtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Vui lòng nhập mật khẩu");
                TxtPassword.Focus();
                return;
            }

            try
            {
                var user = _authService.Login(username, password);

                MessageBox.Show(
                    $"Xin chào, {user.FullName}!\nĐăng nhập thành công.",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowError(ex.Message);
                TxtPassword.Clear();
                TxtPassword.Focus();
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
        }
    }
}