using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QLKhoHang.Models
{
    public class CT_PhieuNhap
    {
        [StringLength(10)]
        [Required]
        public string MaPN { get; set; }

        [ForeignKey("MaPN")]
        [ValidateNever]   // ❗ Không validate navigation
        public PhieuNhap PhieuNhap { get; set; }

        [StringLength(10)]
        [Required]
        public string MaHang { get; set; }

        [ForeignKey("MaHang")]
        [ValidateNever]   // ❗ Không validate navigation
        public HangHoa HangHoa { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaNhap { get; set; }
    }
}
