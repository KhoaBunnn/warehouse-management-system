using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhoHang.Data;
using QLKhoHang.Models;
using System.Linq;

namespace QLKhoHang.Controllers
{
    public class CT_PhieuNhapController : Controller
    {
        private readonly QLKhoHangContext _context;

        public CT_PhieuNhapController(QLKhoHangContext context)
        {
            _context = context;
        }

        // ====== THÊM CHI TIẾT PHIẾU NHẬP ======
        public IActionResult Create(string id)   // id = MaPN
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            ViewBag.MaPN = id;
            ViewBag.HangHoa = _context.HangHoa.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CT_PhieuNhap ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaPN = ct.MaPN;
                ViewBag.HangHoa = _context.HangHoa.ToList();
                return View(ct);
            }

            // Kiểm tra tồn tại phiếu nhập
            var phieu = _context.PhieuNhap.FirstOrDefault(p => p.MaPN == ct.MaPN);
            if (phieu == null)
                return NotFound();

            // Kiểm tra hàng hóa
            var hangHoa = _context.HangHoa.FirstOrDefault(h => h.MaHang == ct.MaHang);
            if (hangHoa == null)
            {
                ModelState.AddModelError("", "Hàng hóa không tồn tại!");
                ViewBag.MaPN = ct.MaPN;
                ViewBag.HangHoa = _context.HangHoa.ToList();
                return View(ct);
            }

            // Nếu hàng đã có trong phiếu => cộng dồn
            var ctTonTai = _context.CT_PhieuNhap
                                   .FirstOrDefault(x => x.MaPN == ct.MaPN && x.MaHang == ct.MaHang);

            if (ctTonTai != null)
            {
                ctTonTai.SoLuong += ct.SoLuong;
                ctTonTai.DonGiaNhap = ct.DonGiaNhap;
            }
            else
            {
                _context.CT_PhieuNhap.Add(ct);
            }

            // Cộng số lượng tồn kho
            hangHoa.SoLuongTon += ct.SoLuong;

            _context.SaveChanges();

            return RedirectToAction("Details", "PhieuNhap", new { id = ct.MaPN });
        }



        // ====== XÓA CHI TIẾT PHIẾU NHẬP ======
        [HttpPost]
        public IActionResult Delete(string maPN, string maHang)
        {
            if (string.IsNullOrEmpty(maPN) || string.IsNullOrEmpty(maHang))
                return NotFound();

            var ct = _context.CT_PhieuNhap
                             .FirstOrDefault(x => x.MaPN == maPN && x.MaHang == maHang);

            if (ct == null)
                return NotFound();

            var hangHoa = _context.HangHoa
                                  .FirstOrDefault(h => h.MaHang == maHang);

            if (hangHoa != null)
            {
                hangHoa.SoLuongTon -= ct.SoLuong;

                if (hangHoa.SoLuongTon < 0)
                    hangHoa.SoLuongTon = 0;
            }

            _context.CT_PhieuNhap.Remove(ct);
            _context.SaveChanges();

            return RedirectToAction("Details", "PhieuNhap", new { id = maPN });
        }
    }
}
