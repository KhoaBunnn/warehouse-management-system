using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLKhoHang.Models
{
    public class PhieuNhap
    {
        [Key]
        [StringLength(10)]
        public string MaPN { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày nhập")]
        public DateTime NgayNhap { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        [StringLength(10)]
        public string MaNV { get; set; }

        [ForeignKey(nameof(MaNV))]
        public virtual NhanVien? NhanVien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        [StringLength(10)]
        public string MaNCC { get; set; }

        [ForeignKey(nameof(MaNCC))]
        public virtual NhaCungCap? NhaCungCap { get; set; }

        public virtual ICollection<CT_PhieuNhap>? CT_PhieuNhap { get; set; }
    }
}
