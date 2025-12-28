# HỆ THỐNG QUẢN LÝ TRANG BỊ (QLTB) v2.0

## 📋 Tổng quan

Hệ thống quản lý trang bị quân sự với các tính năng:
- ✅ Quản lý danh sách trang bị (CRUD đầy đủ)
- ✅ Hệ thống đăng nhập bảo mật (PBKDF2 encryption)
- ✅ Quét QR Code tích hợp Serial CDC
- ✅ Thống kê tổng hợp
- ✅ Audit logging đầy đủ
- ✅ Database SQLite với Entity Framework Core
- ✅ Giao diện WPF hiện đại

## 🔧 Yêu cầu hệ thống

- Windows 10/11
- .NET Framework 4.8
- Visual Studio 2019 hoặc mới hơn
- 100MB dung lượng ổ đĩa

## 📦 Cài đặt

### Bước 1: Clone repository
```bash
git clone https://github.com/tuan23h/qltb.git
cd qltb
```

### Bước 2: Restore NuGet packages
```bash
nuget restore qltb.sln
```

### Bước 3: Build solution
```bash
msbuild qltb.sln /p:Configuration=Release
```

### Bước 4: Chạy ứng dụng
```bash
cd qltb\bin\Release
qltb.exe
```

## 🔑 Thông tin đăng nhập mặc định

**Admin:**
- Username: `admin`
- Password: `Admin@123`

**User:**
- Username: `user`
- Password: `User@123`

## 📁 Cấu trúc dự án

```
qltb/
├── Assets/
│   └── Styles/
│       └── Theme.xaml          # Theme màu sắc
├── Data/
│   ├── AppDbContext.cs         # EF Core DbContext
│   └── DbInitializer.cs        # Khởi tạo database
├── Models/
│   ├── User.cs                 # Model user với mã hóa mật khẩu
│   ├── EquipmentItem.cs        # Model trang bị
│   ├── EquipmentSpec.cs        # Thông số kỹ thuật
│   ├── TacticalFeature.cs      # Tính năng kỹ - chiến thuật
│   ├── IssueHistory.cs         # Lịch sử cấp phát
│   └── AuditLog.cs             # Log audit
├── Services/
│   ├── AuthService.cs          # Xác thực & phân quyền
│   └── EquipmentService.cs     # CRUD trang bị
├── ViewModels/
│   ├── MainViewModel.cs        # ViewModel chính
│   ├── EquipmentListViewModel.cs
│   └── EquipmentDetailViewModel.cs
├── Views/
│   ├── LoginView.xaml          # Màn hình đăng nhập
│   ├── MainWindow.xaml         # Cửa sổ chính
│   ├── EquipmentListView.xaml  # Danh sách trang bị
│   ├── EquipmentEditDialog.xaml # Dialog thêm/sửa
│   ├── EquipmentDetailWindow.xaml # Chi tiết
│   ├── ScanView.xaml           # Quét QR
│   └── StatisticsView.xaml     # Thống kê
├── Hardware/
│   ├── SerialCdcService.cs     # Dịch vụ Serial CDC
│   └── QrScanEventArgs.cs
└── Utils/
    └── RelayCommand.cs         # ICommand implementation
```

## 🔒 Bảo mật

### 1. Mã hóa mật khẩu
- Sử dụng **PBKDF2** với 10,000 iterations
- Salt ngẫu nhiên 256-bit cho mỗi mật khẩu
- Hash 256-bit kết quả

### 2. Database
- SQLite được lưu tại `%LOCALAPPDATA%\QLTB\qltb.db`
- Sử dụng **Parameterized Queries** (Entity Framework Core)
- Không có SQL injection vulnerability

### 3. Validation
- Input validation cho tất cả fields
- Data Annotations trên Models
- Error handling đầy đủ

### 4. Audit Logging
- Tất cả thao tác CRUD được ghi log
- Log đăng nhập/đăng xuất
- Lưu timestamp và username

## 🚀 Tính năng chính

### 1. Quản lý Trang bị
- ➕ Thêm trang bị mới
- ✏️ Sửa thông tin
- 🗑️ Xóa trang bị
- 🔍 Tìm kiếm theo tên, QR Code, vị trí
- 📄 Xem chi tiết đầy đủ

### 2. Phân loại
- 🔫 Vũ khí (Weapon)
- 💣 Đạn dược (Ammunition)  
- 🎒 Trang bị khác (Gear)

### 3. Thông tin chi tiết
- Thông số kỹ thuật (cỡ đạn, khối lượng, tầm bắn...)
- Tính năng kỹ - chiến thuật
- Trạng thái và vị trí
- Lịch sử cập nhật

### 4. Thống kê
- Tổng số trang bị theo loại
- Thống kê theo trạng thái
- Dashboard trực quan

### 5. Quét QR Code
- Tích hợp Serial CDC
- Tự động tra cứu thông tin
- Hiển thị kết quả real-time

## 🛠️ Cấu hình

### Kết nối Serial (QR Scanner)
Mở file `Hardware/SerialCdcService.cs` và thay đổi COM port:

```csharp
public void Start()
{
    Start("COM3"); // Thay đổi COM port tại đây
}
```

### Thay đổi đường dẫn Database
Mở file `Data/AppDbContext.cs`:

```csharp
private static readonly string DbPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "QLTB", "qltb.db");
```

## 🐛 Xử lý lỗi

### Lỗi: "Database is locked"
**Giải pháp:** Đảm bảo đóng tất cả `AppDbContext` bằng `using` statement hoặc `Dispose()`.

### Lỗi: "COM port already in use"
**Giải pháp:** Kiểm tra COM port đang sử dụng, đóng các ứng dụng khác đang dùng port.

### Lỗi: "User not found"
**Giải pháp:** Xóa file database tại `%LOCALAPPDATA%\QLTB\qltb.db` và chạy lại ứng dụng để tạo lại DB mới.

## 📊 Migrations (Nếu cần thay đổi DB Schema)

```bash
# Tạo migration mới
dotnet ef migrations add MigrationName --project qltb

# Apply migration
dotnet ef database update --project qltb
```

## 🔄 Cập nhật phiên bản

### Từ v1.0 lên v2.0:
1. Backup database cũ: `%LOCALAPPDATA%\QLTB\qltb.db`
2. Cài đặt phiên bản mới
3. Database mới sẽ được tạo tự động với cấu trúc cải tiến

## 🤝 Đóng góp

1. Fork repository
2. Tạo branch mới: `git checkout -b feature/AmazingFeature`
3. Commit changes: `git commit -m 'Add AmazingFeature'`
4. Push to branch: `git push origin feature/AmazingFeature`
5. Tạo Pull Request

## 📝 License

Dự án này được phát triển cho mục đích quản lý nội bộ.

## 👨‍💻 Tác giả

**Team QLTB Development**

## 📞 Hỗ trợ

- Email: support@qltb.local
- Issues: https://github.com/tuan23h/qltb/issues

---

**Lưu ý bảo mật:**
- ⚠️ Thay đổi mật khẩu mặc định sau lần đăng nhập đầu tiên
- ⚠️ Không chia sẻ thông tin đăng nhập
- ⚠️ Backup database định kỳ
- ⚠️ Giữ phần mềm luôn được cập nhật