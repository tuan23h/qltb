using System;
using System.Linq;
using qltb.Data;
using qltb.Models;

namespace qltb.Services
{
    public class AuthService
    {
        private static User _currentUser;

        public static User CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public static bool IsAuthenticated => CurrentUser != null;

        public static bool IsAdmin => CurrentUser?.Role == "Admin";

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Tên đăng nhập và mật khẩu không được để trống");
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u =>
                    u.Username.ToLower() == username.ToLower() && u.IsActive);

                if (user == null)
                {
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng");
                }

                if (!User.VerifyPassword(password, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng");
                }

                // Cập nhật thời gian đăng nhập
                user.LastLoginAt = DateTime.Now;
                db.SaveChanges();

                // Log audit
                db.AuditLogs.Add(new AuditLog
                {
                    Action = $"Đăng nhập thành công",
                    Time = DateTime.Now,
                    User = username
                });
                db.SaveChanges();

                CurrentUser = user;
                return user;
            }
        }

        public void Logout()
        {
            if (CurrentUser != null)
            {
                using (var db = new AppDbContext())
                {
                    db.AuditLogs.Add(new AuditLog
                    {
                        Action = "Đăng xuất",
                        Time = DateTime.Now,
                        User = CurrentUser.Username
                    });
                    db.SaveChanges();
                }
            }

            CurrentUser = null;
        }

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            if (CurrentUser == null)
                throw new UnauthorizedAccessException("Chưa đăng nhập");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự");

            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(CurrentUser.Id);

                if (user == null || !User.VerifyPassword(oldPassword, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Mật khẩu cũ không đúng");
                }

                user.PasswordHash = User.HashPassword(newPassword);
                db.SaveChanges();

                db.AuditLogs.Add(new AuditLog
                {
                    Action = "Đổi mật khẩu",
                    Time = DateTime.Now,
                    User = user.Username
                });
                db.SaveChanges();

                return true;
            }
        }
    }
}