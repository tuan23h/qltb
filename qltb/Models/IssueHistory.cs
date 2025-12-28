using System;

namespace qltb.Models
{
    public class IssueHistory
    {
        public int Id { get; set; }
        public int EquipmentItemId { get; set; }
        public string IssuedTo { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Note { get; set; }
    }
}
