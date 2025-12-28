using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using qltb.Data;
using qltb.Models;

namespace qltb.Services
{
    public class EquipmentService : IDisposable
    {
        private readonly AppDbContext _db;

        public EquipmentService()
        {
            _db = new AppDbContext();
        }

        public List<EquipmentItem> GetAll()
        {
            return _db.EquipmentItems
                .Include(e => e.TacticalFeatures)
                .OrderBy(e => e.Name)
                .ToList();
        }

        public List<EquipmentItem> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            keyword = keyword.ToLower().Trim();
            return _db.EquipmentItems
                .Include(e => e.TacticalFeatures)
                .Where(e => e.Name.ToLower().Contains(keyword) ||
                           e.QRCode.ToLower().Contains(keyword) ||
                           e.Location.ToLower().Contains(keyword))
                .OrderBy(e => e.Name)
                .ToList();
        }

        public EquipmentItem GetById(int id)
        {
            return _db.EquipmentItems
                .Include(e => e.TacticalFeatures)
                .FirstOrDefault(e => e.Id == id);
        }

        public EquipmentItem GetByQRCode(string qrCode)
        {
            if (string.IsNullOrWhiteSpace(qrCode))
                return null;

            return _db.EquipmentItems
                .Include(e => e.TacticalFeatures)
                .FirstOrDefault(e => e.QRCode == qrCode);
        }

        public EquipmentItem Add(EquipmentItem item)
        {
            ValidateEquipment(item);

            // Kiểm tra trùng QR Code
            if (_db.EquipmentItems.Any(e => e.QRCode == item.QRCode))
            {
                throw new InvalidOperationException($"QR Code '{item.QRCode}' đã tồn tại");
            }

            item.CreatedAt = DateTime.Now;
            item.CreatedBy = AuthService.CurrentUser?.Username ?? "System";

            _db.EquipmentItems.Add(item);
            _db.SaveChanges();

            LogAction($"Thêm trang bị: {item.Name} ({item.QRCode})");

            return item;
        }

        public EquipmentItem Update(EquipmentItem item)
        {
            ValidateEquipment(item);

            var existing = _db.EquipmentItems.Find(item.Id);
            if (existing == null)
            {
                throw new InvalidOperationException("Không tìm thấy trang bị");
            }

            // Kiểm tra trùng QR Code (trừ chính nó)
            if (_db.EquipmentItems.Any(e => e.QRCode == item.QRCode && e.Id != item.Id))
            {
                throw new InvalidOperationException($"QR Code '{item.QRCode}' đã tồn tại");
            }

            existing.QRCode = item.QRCode;
            existing.Name = item.Name;
            existing.Category = item.Category;
            existing.Quantity = item.Quantity;
            existing.Status = item.Status;
            existing.Location = item.Location;
            existing.Spec = item.Spec;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = AuthService.CurrentUser?.Username ?? "System";

            // Cập nhật TacticalFeatures
            _db.Entry(existing).Collection(e => e.TacticalFeatures).Load();
            existing.TacticalFeatures.Clear();
            if (item.TacticalFeatures != null)
            {
                foreach (var feature in item.TacticalFeatures)
                {
                    existing.TacticalFeatures.Add(feature);
                }
            }

            _db.SaveChanges();
            LogAction($"Cập nhật trang bị: {item.Name} ({item.QRCode})");

            return existing;
        }

        public bool Delete(int id)
        {
            var item = _db.EquipmentItems.Find(id);
            if (item == null)
                return false;

            // Kiểm tra xem có đang được cấp phát không
            var hasActiveIssue = _db.IssueHistories.Any(h =>
                h.EquipmentItemId == id && h.ReturnDate == null);

            if (hasActiveIssue)
            {
                throw new InvalidOperationException("Không thể xóa trang bị đang được cấp phát");
            }

            _db.EquipmentItems.Remove(item);
            _db.SaveChanges();

            LogAction($"Xóa trang bị: {item.Name} ({item.QRCode})");

            return true;
        }

        public List<EquipmentItem> GetByCategory(EquipmentCategory category)
        {
            return _db.EquipmentItems
                .Include(e => e.TacticalFeatures)
                .Where(e => e.Category == category)
                .OrderBy(e => e.Name)
                .ToList();
        }

        public List<EquipmentItem> GetByStatus(string status)
        {
            return _db.EquipmentItems
                .Include(e => e.TacticalFeatures)
                .Where(e => e.Status == status)
                .OrderBy(e => e.Name)
                .ToList();
        }

        public Dictionary<string, int> GetStatistics()
        {
            return new Dictionary<string, int>
            {
                ["TotalItems"] = _db.EquipmentItems.Count(),
                ["Weapons"] = _db.EquipmentItems.Count(e => e.Category == EquipmentCategory.Weapon),
                ["Ammunition"] = _db.EquipmentItems.Count(e => e.Category == EquipmentCategory.Ammunition),
                ["Gear"] = _db.EquipmentItems.Count(e => e.Category == EquipmentCategory.Gear),
                ["Available"] = _db.EquipmentItems.Count(e => e.Status == "Sẵn sàng"),
                ["InUse"] = _db.EquipmentItems.Count(e => e.Status == "Đang sử dụng"),
                ["Maintenance"] = _db.EquipmentItems.Count(e => e.Status == "Bảo trì")
            };
        }

        private void ValidateEquipment(EquipmentItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var validationContext = new ValidationContext(item);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(item, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException(errors);
            }

            if (string.IsNullOrWhiteSpace(item.Status))
                item.Status = "Sẵn sàng";

            if (string.IsNullOrWhiteSpace(item.Location))
                item.Location = "Chưa xác định";
        }

        private void LogAction(string action)
        {
            _db.AuditLogs.Add(new AuditLog
            {
                Action = action,
                Time = DateTime.Now,
                User = AuthService.CurrentUser?.Username ?? "System"
            });
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}