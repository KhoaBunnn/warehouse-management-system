using System;

namespace QLKhoHang.Models
{
    public class LichSuSuaPhieu
    {
        public int ID { get; set; }
        public string MaPhieu { get; set; }
        public string LoaiPhieu { get; set; }   // Nhap / Xuat
        public string? MaHang { get; set; }
        public string TruongSua { get; set; }
        public string GiaTriCu { get; set; }
        public string GiaTriMoi { get; set; }
        public DateTime ThoiGian { get; set; }
        public string NguoiSua { get; set; }
    }
}
