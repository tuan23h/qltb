using System.Collections.Generic;
using System.Linq;
using qltb.Data;
using qltb.Models;

namespace qltb.Services
{
    public class EquipmentService
    {
        private readonly AppDbContext _db = new AppDbContext();

        public List<EquipmentItem> GetAll()
        {
            return _db.EquipmentItems.ToList();
        }

        public EquipmentItem GetByQRCode(string qrCode)
        {
            using (var db = new AppDbContext())
            {
                return db.EquipmentItems
                         .FirstOrDefault(e => e.QRCode == qrCode);
            }
        }
    }
}
