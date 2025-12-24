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

            // Kiểm tra phiếu nhập
            var phieu = _context.PhieuNhap.FirstOrDefault(p => p.MaPN == ct.MaPN);
            if (phieu == null)
            {
                ModelState.AddModelError("", "Phiếu nhập không tồn tại!");
                ViewBag.MaPN = ct.MaPN;
                ViewBag.HangHoa = _context.HangHoa.ToList();
                return View(ct);
            }

            // Kiểm tra hàng hóa
            var hangHoa = _context.HangHoa.FirstOrDefault(h => h.MaHang == ct.MaHang);
            if (hangHoa == null)
            {
                ModelState.AddModelError("", "Hàng hóa không tồn tại!");
                ViewBag.MaPN = ct.MaPN;
                ViewBag.HangHoa = _context.HangHoa.ToList();
                return View(ct);
            }

            // Nếu hàng hóa đã tồn tại trong phiếu
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

            // Cập nhật tồn kho
            hangHoa.SoLuongTon += ct.SoLuong;

            try
            {
                _context.SaveChanges();   // ❗ Nếu lỗi xảy ra → catch
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể lưu dữ liệu. Lỗi: " + ex.Message);

                ViewBag.MaPN = ct.MaPN;
                ViewBag.HangHoa = _context.HangHoa.ToList();

                return View(ct); // HIỆN LỖI RA VIEW
            }

            return RedirectToAction("Details", "PhieuNhap", new { id = ct.MaPN });
        }

        // ====== GET: SỬA CHI TIẾT PHIẾU NHẬP ======
        public IActionResult Edit(string maPN, string maHang)
        {
            if (string.IsNullOrEmpty(maPN) || string.IsNullOrEmpty(maHang))
                return NotFound();

            var ct = _context.CT_PhieuNhap
                             .Include(x => x.HangHoa)
                             .FirstOrDefault(x => x.MaPN == maPN && x.MaHang == maHang);

            if (ct == null)
                return NotFound();

            ViewBag.HangHoa = _context.HangHoa.ToList();
            return View(ct);
        }



        // ====== POST: SỬA CHI TIẾT PHIẾU NHẬP ======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CT_PhieuNhap ctMoi)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.HangHoa = _context.HangHoa.ToList();
                return View(ctMoi);
            }

            var ctCu = _context.CT_PhieuNhap
                               .FirstOrDefault(x => x.MaPN == ctMoi.MaPN && x.MaHang == ctMoi.MaHang);

            if (ctCu == null)
                return NotFound();


            // ============================
            // CẬP NHẬT TỒN KHO ĐÚNG CHUẨN
            // ============================

            var hangHoa = _context.HangHoa.FirstOrDefault(h => h.MaHang == ctMoi.MaHang);

            if (hangHoa == null)
                return NotFound();

            // Trừ số lượng cũ khỏi tồn kho
            hangHoa.SoLuongTon -= ctCu.SoLuong;

            // Cộng số lượng mới vào tồn kho
            hangHoa.SoLuongTon += ctMoi.SoLuong;

            if (hangHoa.SoLuongTon < 0)
                hangHoa.SoLuongTon = 0;

            // ============================
            // Cập nhật dữ liệu chi tiết
            // ============================

            ctCu.SoLuong = ctMoi.SoLuong;
            ctCu.DonGiaNhap = ctMoi.DonGiaNhap;

            _context.SaveChanges();

            return RedirectToAction("Details", "PhieuNhap", new { id = ctMoi.MaPN });
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
