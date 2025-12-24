using QLKhoHang.Models;

public class HopDong
{
    public int Id { get; set; }
    public string SoHopDong { get; set; }
    public string LoaiHopDong { get; set; }
    public DateTime NgayKy { get; set; }

    public string? MaNCC { get; set; }
    public NhaCungCap? NhaCungCap { get; set; }

    public string? MaKH { get; set; }
    public KhachHang? KhachHang { get; set; }

    public decimal TongTien { get; set; }
    public string? GhiChu { get; set; }

    public List<ChiTietHopDong> ChiTietHopDongs { get; set; }
}
