using System.Linq;
using qltb.Models;

namespace qltb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext db)
        {
            db.Database.EnsureCreated();

            if (db.EquipmentItems.Any())
                return;

            db.EquipmentItems.Add(new EquipmentItem
            {
                QRCode = "AK74-001",
                Name = "Súng AK-74",
                Category = EquipmentCategory.Weapon,
                Quantity = 50,
                Spec = new EquipmentSpec
                {
                    Caliber = "5.45mm",
                    Weight = "3.3kg",
                    Range = "500m",
                    Material = "Thép"
                }
            });

            db.SaveChanges();
        }
    }
}
