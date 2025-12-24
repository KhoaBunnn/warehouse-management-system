using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLKhoHang.Models;

namespace QLKhoHang.Data
{
    public class QLKhoHangContext : IdentityDbContext<IdentityUser>
    {
        public QLKhoHangContext(DbContextOptions<QLKhoHangContext> options)
            : base(options)
        {
        }

        public DbSet<Kho> Kho { get; set; }
        public DbSet<LoaiHang> LoaiHang { get; set; }
        public DbSet<HangHoa> HangHoa { get; set; }
        public DbSet<NhanVien> NhanVien { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<NhaCungCap> NhaCungCap { get; set; }
        public DbSet<PhieuNhap> PhieuNhap { get; set; }
        public DbSet<CT_PhieuNhap> CT_PhieuNhap { get; set; }
        public DbSet<PhieuXuat> PhieuXuat { get; set; }
        public DbSet<CT_PhieuXuat> CT_PhieuXuat { get; set; }
        public DbSet<LichSuSuaPhieu> LichSuSuaPhieu { get; set; }
        public DbSet<BaoCaoNhapHang> BaoCaoNhapHang { get; set; }
        public DbSet<BaoCaoXuatHang> BaoCaoXuatHang { get; set; }
        public DbSet<BaoCaoTonKho> BaoCaoTonKho { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // **Bắt buộc** khi dùng IdentityDbContext

            modelBuilder.Entity<CT_PhieuNhap>()
                .HasKey(c => new { c.MaPN, c.MaHang });

            modelBuilder.Entity<CT_PhieuXuat>()
                .HasKey(c => new { c.MaPX, c.MaHang });

            modelBuilder.Entity<CT_PhieuNhap>()
                .Property(p => p.MaPN)
                .ValueGeneratedNever();

            modelBuilder.Entity<CT_PhieuXuat>()
                .Property(p => p.MaPX)
                .ValueGeneratedNever();

            // Tắt OUTPUT CLAUSE cho bảng có trigger
            modelBuilder.Entity<PhieuNhap>().ToTable(tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<CT_PhieuNhap>().ToTable(tb => tb.UseSqlOutputClause(false));

            modelBuilder.Entity<PhieuXuat>().ToTable(tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<CT_PhieuXuat>().ToTable(tb => tb.UseSqlOutputClause(false));

            modelBuilder.Entity<HangHoa>().ToTable(tb => tb.UseSqlOutputClause(false));
        }
    }
}
