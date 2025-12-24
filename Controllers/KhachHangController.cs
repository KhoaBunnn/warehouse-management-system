using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhoHang.Data;
using QLKhoHang.Models;

namespace QLKhoHang.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly QLKhoHangContext _context;

        public KhachHangController(QLKhoHangContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var list = _context.KhachHang.ToList();
            return View(list);
        }

        private string GenerateMaKH()
        {
            var last = _context.KhachHang
                .OrderByDescending(x => x.MaKH)
                .Select(x => x.MaKH)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(last))
                return "KH001";

            int number = int.Parse(last.Substring(2)) + 1;
            return "KH" + number.ToString("D3");
        }

        public IActionResult Create()
        {
            ViewBag.MaKH = GenerateMaKH();
            return View();
        }

        [HttpPost]
        public IActionResult Create(KhachHang kh)
        {
            // ⛔ STOP — Không validate navigation property
            ModelState.Remove("PhieuXuat");

            if (ModelState.IsValid)
            {
                if (kh.MaKH == null || kh.MaKH == "")
                    kh.MaKH = GenerateMaKH();

                _context.KhachHang.Add(kh);
                _context.SaveChanges();
                TempData["Success"] = "Thêm khách hàng thành công.";
                return RedirectToAction("Index");
            }

            // Nếu lỗi quay về form
            ViewBag.MaKH = kh.MaKH;
            return View(kh);
        }

        public IActionResult Edit(string id)
        {
            var kh = _context.KhachHang.Find(id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost]
        public IActionResult Edit(KhachHang kh)
        {
            ModelState.Remove("PhieuXuat");

            if (ModelState.IsValid)
            {
                _context.KhachHang.Update(kh);
                _context.SaveChanges();
                TempData["Success"] = "Cập nhật khách hàng thành công.";
                return RedirectToAction("Index");
            }

            return View(kh);
        }
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var kh = _context.KhachHang
                .Include(x => x.PhieuXuat)
                    .ThenInclude(px => px.NhanVien)
                .FirstOrDefault(x => x.MaKH == id);

            if (kh == null)
                return NotFound();

            return View(kh);
        }
        // ================================
        // 🟥 GET: Delete
        // ================================
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var kh = _context.KhachHang
                .Include(x => x.PhieuXuat)
                .FirstOrDefault(x => x.MaKH == id);

            if (kh == null)
                return NotFound();

            return View(kh);
        }

        // ================================
        // 🟥 POST: DeleteConfirmed
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string MaKH)
        {
            var kh = _context.KhachHang
                .Include(x => x.PhieuXuat)
                .FirstOrDefault(x => x.MaKH == MaKH);

            if (kh == null)
                return NotFound();

            // ❗ Không cho xóa nếu KH có phiếu xuất
            if (kh.PhieuXuat != null && kh.PhieuXuat.Any())
            {
                TempData["Error"] = "Không thể xóa khách hàng vì đã phát sinh phiếu xuất.";
                return RedirectToAction("Delete", new { id = MaKH });
            }

            _context.KhachHang.Remove(kh);
            _context.SaveChanges();
            TempData["Success"] = "Xóa khách hàng thành công.";
            return RedirectToAction("Index");
        }

    }
}
