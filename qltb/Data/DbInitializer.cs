using System;
using System.Linq;
using qltb.Models;

namespace qltb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext db)
        {
            db.Database.EnsureCreated();

            // Tạo user admin mặc định nếu chưa có
            if (!db.Users.Any())
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = User.HashPassword("Admin@123"),
                    FullName = "Quản trị viên",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var normalUser = new User
                {
                    Username = "user",
                    PasswordHash = User.HashPassword("User@123"),
                    FullName = "Người dùng",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                db.Users.AddRange(adminUser, normalUser);
                db.SaveChanges();
            }

            // Tạo dữ liệu mẫu nếu chưa có
            if (!db.EquipmentItems.Any())
            {
                var weapons = new[]
                {
                    new EquipmentItem
                    {
                        QRCode = "AK74-001",
                        Name = "Súng trường AK-74",
                        Category = EquipmentCategory.Weapon,
                        Quantity = 50,
                        Status = "Sẵn sàng",
                        Location = "Kho A - Kệ 1",
                        CreatedBy = "admin",
                        Spec = new EquipmentSpec
                        {
                            Caliber = "5.45×39mm",
                            Weight = "3.3kg",
                            Range = "500m",
                            Material = "Thép hợp kim"
                        },
                        TacticalFeatures = new System.Collections.Generic.List<TacticalFeature>
                        {
                            new TacticalFeature { Description = "Độ chính xác cao ở tầm trung" },
                            new TacticalFeature { Description = "Giật yếu, dễ điều khiển" }
                        }
                    },
                    new EquipmentItem
                    {
                        QRCode = "M16-002",
                        Name = "Súng trường M16A1",
                        Category = EquipmentCategory.Weapon,
                        Quantity = 30,
                        Status = "Sẵn sàng",
                        Location = "Kho A - Kệ 2",
                        CreatedBy = "admin",
                        Spec = new EquipmentSpec
                        {
                            Caliber = "5.56×45mm NATO",
                            Weight = "3.26kg",
                            Range = "550m",
                            Material = "Nhôm hợp kim"
                        },
                        TacticalFeatures = new System.Collections.Generic.List<TacticalFeature>
                        {
                            new TacticalFeature { Description = "Tốc độ đạn cao, xuyên giáp tốt" },
                            new TacticalFeature { Description = "Độ chính xác cao ở mọi khoảng cách" }
                        }
                    }
                };

                var ammunition = new EquipmentItem
                {
                    QRCode = "AMMO-545-001",
                    Name = "Đạn 5.45×39mm",
                    Category = EquipmentCategory.Ammunition,
                    Quantity = 10000,
                    Status = "Sẵn sàng",
                    Location = "Kho B - Két sắt 1",
                    CreatedBy = "admin",
                    Spec = new EquipmentSpec
                    {
                        Caliber = "5.45×39mm",
                        Weight = "10.2g/viên",
                        Range = "N/A",
                        Material = "Đầu đạn thép bọc đồng"
                    }
                };

                var gear = new EquipmentItem
                {
                    QRCode = "VEST-001",
                    Name = "Áo giáp chống đạn cấp III",
                    Category = EquipmentCategory.Gear,
                    Quantity = 100,
                    Status = "Sẵn sàng",
                    Location = "Kho C - Giá treo 1",
                    CreatedBy = "admin",
                    Spec = new EquipmentSpec
                    {
                        Weight = "4.5kg",
                        Material = "Kevlar composite",
                        Range = "N/A"
                    },
                    TacticalFeatures = new System.Collections.Generic.List<TacticalFeature>
                    {
                        new TacticalFeature { Description = "Chống đạn cỡ 7.62mm từ khoảng cách 10m" },
                        new TacticalFeature { Description = "Thoáng khí, linh động" }
                    }
                };

                db.EquipmentItems.AddRange(weapons);
                db.EquipmentItems.Add(ammunition);
                db.EquipmentItems.Add(gear);
                db.SaveChanges();

                // Tạo audit log
                db.AuditLogs.Add(new AuditLog
                {
                    Action = "Khởi tạo dữ liệu hệ thống",
                    Time = DateTime.Now,
                    User = "System"
                });
                db.SaveChanges();
            }
        }
    }
}