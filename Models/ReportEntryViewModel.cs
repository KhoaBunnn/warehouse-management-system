using System;

namespace QLKhoHang.Models
{
    public class ReportEntryViewModel
    {
        public string Type { get; set; }
        public string RefId { get; set; }
        public DateTime Ngay { get; set; }
        public string MaHH { get; set; }
        public string TenHang { get; set; }
        public string MaKho { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
    }
}
