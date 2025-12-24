
namespace QLKhoHang.Models
{
    public class ChiTietHopDong
    {
        public int Id { get; set; }

        public int HopDongId { get; set; }
        public HopDong HopDong { get; set; }

        public int MaHang { get; set; }
        public HangHoa HangHoa { get; set; }

        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}