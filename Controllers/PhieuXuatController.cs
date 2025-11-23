using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhoHang.Data;
using QLKhoHang.Models;
using System.Linq;

namespace QLKhoHang.Controllers
{
    public class PhieuXuatController : Controller
    {
        private readonly QLKhoHangContext _context;

        public PhieuXuatController(QLKhoHangContext context)
        {
            _context = context;
        }

        // ==========================
        // Index
        // ==========================
        public IActionResult Index()
        {
            var data = _context.PhieuXuat
                .Include(p => p.NhanVien)
                .Include(p => p.KhachHang)
                .OrderByDescending(p => p.NgayXuat)
                .ToList();
            return View(data);
        }

        // ==========================
        // Tạo mã tự động
        // ==========================
        private string GenerateMaPX()
        {
            var last = _context.PhieuXuat
                .OrderByDescending(x => x.MaPX)
                .Select(x => x.MaPX)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(last))
                return "PX001";

            int num = 1;
            if (last.Length > 2 && int.TryParse(last.Substring(2), out var n))
                num = n + 1;
            return "PX" + num.ToString("D3");
        }

        // ==========================
        // GET: Create (header)
        // ==========================
        public IActionResult Create()
        {
            ViewBag.NewId = GenerateMaPX();
            ViewBag.NhanVien = _context.NhanVien.ToList();
            ViewBag.KhachHang = _context.KhachHang.ToList();
            return View(new PhieuXuat { NgayXuat = System.DateTime.Now });
        }

        // ==========================
        // POST: Create (header)
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhieuXuat px)
        {
            // Remove validation of navigation/collection properties (model unchanged)
            ModelState.Remove("CT_PhieuXuat");
            ModelState.Remove("NhanVien");
            ModelState.Remove("KhachHang");

            // Log ModelState errors for debugging
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"[MODEL ERROR] KEY={key} | ERROR={error.ErrorMessage}");
                }
            }

            if (string.IsNullOrEmpty(px.MaPX))
                px.MaPX = GenerateMaPX();

            if (ModelState.IsValid)
            {
                _context.PhieuXuat.Add(px);
                _context.SaveChanges();
                Console.WriteLine("[OK] Lưu phiếu xuất thành công: " + px.MaPX);
                return RedirectToAction("Details", new { id = px.MaPX });
            }

            // nếu invalid: load lại dropdowns
            ViewBag.NewId = px.MaPX;
            ViewBag.NhanVien = _context.NhanVien.ToList();
            ViewBag.KhachHang = _context.KhachHang.ToList();
            return View(px);
        }

        // ==========================
        // Details (show header + CTs + form add CT)
        // ==========================
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var px = _context.PhieuXuat
                .Include(p => p.NhanVien)
                .Include(p => p.KhachHang)
                .Include(p => p.CT_PhieuXuat)
                    .ThenInclude(ct => ct.HangHoa)
                .FirstOrDefault(p => p.MaPX == id);

            if (px == null) return NotFound();

            ViewBag.HangHoa = _context.HangHoa.ToList();
            return View(px);
        }

        // ==========================
        // POST: AddDetail (thêm 1 dòng CT_PhieuXuat)
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDetail(string maPX, string maHang, int soLuong, decimal donGia)
        {
            if (string.IsNullOrEmpty(maPX) || string.IsNullOrEmpty(maHang))
            {
                TempData["Error"] = "Thiếu thông tin mã phiếu hoặc mã hàng.";
                return RedirectToAction("Details", new { id = maPX });
            }

            // tìm phiếu & hàng
            var px = _context.PhieuXuat.Find(maPX);
            var hang = _context.HangHoa.Find(maHang);

            if (px == null)
            {
                TempData["Error"] = "Phiếu không tồn tại.";
                return RedirectToAction("Index");
            }
            if (hang == null)
            {
                TempData["Error"] = "Hàng hóa không tồn tại.";
                return RedirectToAction("Details", new { id = maPX });
            }

            if (soLuong <= 0)
            {
                TempData["Error"] = "Số lượng phải lớn hơn 0.";
                return RedirectToAction("Details", new { id = maPX });
            }

            if (hang.SoLuongTon < soLuong)
            {
                TempData["Error"] = "Không đủ tồn kho.";
                return RedirectToAction("Details", new { id = maPX });
            }

            // Nếu đã có dòng cùng MaHang cho MaPX thì cộng dồn (hoặc bạn có thể cấm)
            var existing = _context.CT_PhieuXuat.FirstOrDefault(c => c.MaPX == maPX && c.MaHang == maHang);
            if (existing != null)
            {
                existing.SoLuong += soLuong;
                existing.DonGiaXuat = donGia; // cập nhật đơn giá theo lần nhập mới
                // cập nhật tồn kho
                hang.SoLuongTon -= soLuong;
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = maPX });
            }

            var ct = new CT_PhieuXuat
            {
                MaPX = maPX,
                MaHang = maHang,
                SoLuong = soLuong,
                DonGiaXuat = donGia
            };

            // giảm tồn kho
            hang.SoLuongTon -= soLuong;

            _context.CT_PhieuXuat.Add(ct);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = maPX });
        }

        // ==========================
        // GET: DeleteDetail (xóa 1 dòng chi tiết)
        // ==========================
        public IActionResult DeleteDetail(string maPX, string maHang)
        {
            if (string.IsNullOrEmpty(maPX) || string.IsNullOrEmpty(maHang))
                return RedirectToAction("Details", new { id = maPX });

            var ct = _context.CT_PhieuXuat.FirstOrDefault(c => c.MaPX == maPX && c.MaHang == maHang);
            if (ct == null) return RedirectToAction("Details", new { id = maPX });

            // hoàn trả tồn kho
            var hang = _context.HangHoa.Find(ct.MaHang);
            if (hang != null) hang.SoLuongTon += ct.SoLuong;

            _context.CT_PhieuXuat.Remove(ct);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = maPX });
        }

        // ==========================
        // GET: Delete (xóa cả phiếu)
        // ==========================
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var px = _context.PhieuXuat
                .Include(p => p.NhanVien)
                .Include(p => p.KhachHang)
                .Include(p => p.CT_PhieuXuat)
                .FirstOrDefault(p => p.MaPX == id);

            if (px == null) return NotFound();

            return View(px);
        }

        // ==========================
        // POST: DeleteConfirmed
        // ==========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var px = _context.PhieuXuat
                .Include(p => p.CT_PhieuXuat)
                .FirstOrDefault(p => p.MaPX == id);

            if (px == null) return NotFound();

            // hoàn trả tồn kho cho từng chi tiết
            foreach (var ct in px.CT_PhieuXuat)
            {
                var hang = _context.HangHoa.Find(ct.MaHang);
                if (hang != null)
                    hang.SoLuongTon += ct.SoLuong;
            }

            _context.CT_PhieuXuat.RemoveRange(px.CT_PhieuXuat);
            _context.PhieuXuat.Remove(px);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
