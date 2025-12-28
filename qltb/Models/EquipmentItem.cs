using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace qltb.Models
{
    public class EquipmentItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "QR Code là bắt buộc")]
        [StringLength(50)]
        public string QRCode { get; set; }

        [Required(ErrorMessage = "Tên trang bị là bắt buộc")]
        [StringLength(200)]
        public string Name { get; set; }

        public EquipmentCategory Category { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int Quantity { get; set; }

        [StringLength(50)]
        public string Status { get; set; } // Sẵn sàng, Đang sử dụng, Bảo trì, Hỏng

        [StringLength(100)]
        public string Location { get; set; } // Kho A, Kho B, Đơn vị X...

        public EquipmentSpec Spec { get; set; }

        public List<TacticalFeature> TacticalFeatures { get; set; } = new List<TacticalFeature>();

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        // Thuộc tính computed
        public string DisplayName => $"{Name} ({QRCode})";
    }
}