using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLKhoHang.Models.ViewModels
{
    public class HangHoaBaseViewModel
    {
        [Required]
        public string MaHang { get; set; }

        [Required]
        public string TenHang { get; set; }

        public string DonViTinh { get; set; }

        public int SoLuongTon { get; set; }

        public decimal GiaNhap { get; set; }

        public decimal GiaXuat { get; set; }

        [Required]
        public string MaKho { get; set; }

        [Required]
        public string MaLoai { get; set; }

        // ✨ FIX QUAN TRỌNG
        [ValidateNever]
        public IEnumerable<SelectListItem> KhoList { get; set; }

        // ✨ FIX QUAN TRỌNG
        [ValidateNever]
        public IEnumerable<SelectListItem> LoaiHangList { get; set; }
    }
}
