using System;

namespace qltb.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public DateTime Time { get; set; }
        public string User { get; set; }
    }
}
