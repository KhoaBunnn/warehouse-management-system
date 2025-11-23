using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhoHang.Data;
using QLKhoHang.Models;

namespace QLKhoHang.Controllers
{
    public class PhieuNhapController : Controller
    {
        private readonly QLKhoHangContext _context;

        public PhieuNhapController(QLKhoHangContext context)
        {
            _context = context;
        }

        // ============================
        // 🔵 Danh sách
        // ============================
        public IActionResult Index()
        {
            var list = _context.PhieuNhap
                .Include(p => p.NhanVien)
                .Include(p => p.NhaCungCap)
                .ToList();

            return View(list);
        }

        // ============================
        // 🔵 Tạo mã tự động
        // ============================
        private string GenerateMaPN()
        {
            var last = _context.PhieuNhap
                .OrderByDescending(x => x.MaPN)
                .Select(x => x.MaPN)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(last))
                return "PN0001";

            int number = int.Parse(last.Substring(2));
            number++;
            return "PN" + number.ToString("D4");
        }

        // ============================
        // 🔵 GET: Create
        // ============================
        public IActionResult Create()
        {
            ViewBag.MaPN = GenerateMaPN();
            ViewBag.NhanVien = _context.NhanVien.ToList();
            ViewBag.NCC = _context.NhaCungCap.ToList();
            return View();
        }

        // ============================
        // 🔵 POST: Create
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhieuNhap pn)
        {
            if (string.IsNullOrEmpty(pn.MaPN))
                pn.MaPN = GenerateMaPN();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.PhieuNhap.Add(pn);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.MaPN = pn.MaPN;
            ViewBag.NhanVien = _context.NhanVien.ToList();
            ViewBag.NCC = _context.NhaCungCap.ToList();

            return View(pn);
        }

        // ============================
        // 🔵 DETAILS
        // ============================
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var pn = _context.PhieuNhap
                .Include(p => p.NhanVien)
                .Include(p => p.NhaCungCap)
                .Include(p => p.CT_PhieuNhap)
                    .ThenInclude(ct => ct.HangHoa)
                .FirstOrDefault(p => p.MaPN == id);

            if (pn == null)
                return NotFound();

            return View(pn);
        }

        // ============================
        // 🔵 GET: Edit
        // ============================
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var pn = _context.PhieuNhap.Find(id);
            if (pn == null)
                return NotFound();

            ViewBag.NhanVien = _context.NhanVien.ToList();
            ViewBag.NCC = _context.NhaCungCap.ToList();

            return View(pn);
        }

        // ============================
        // 🔵 POST: Edit
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PhieuNhap pn)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.PhieuNhap.Update(pn);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                }
            }

            ViewBag.NhanVien = _context.NhanVien.ToList();
            ViewBag.NCC = _context.NhaCungCap.ToList();

            return View(pn);
        }

        // ============================
        // 🔵 DELETE
        // ============================
        public IActionResult Delete(string id)
        {
            var pn = _context.PhieuNhap.Find(id);
            return View(pn);
        }

        [HttpPost]
        public IActionResult Delete(PhieuNhap pn)
        {
            _context.PhieuNhap.Remove(pn);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
