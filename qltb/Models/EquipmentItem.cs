using System.Collections.Generic;

namespace qltb.Models
{
    public class EquipmentItem
    {
        public int Id { get; set; }

        public string QRCode { get; set; }        // <<< THÊM / ĐỔI
        public string Name { get; set; }

        public EquipmentCategory Category { get; set; }

        public int Quantity { get; set; }

        public EquipmentSpec Spec { get; set; }

        public List<TacticalFeature> TacticalFeatures { get; set; }
            = new List<TacticalFeature>();
    }
}
