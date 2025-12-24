using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhoHang.Data;
using QLKhoHang.Models;

namespace QLKhoHang.Controllers
{
    [Authorize(Roles = "Admin,NhanVienKho,KeToan")]
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
        [Authorize(Roles = "Admin,NhanVienKho")]
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

                    // ==================== 🔥 Lưu lịch sử tạo phiếu ====================
                    _context.LichSuSuaPhieu.Add(new LichSuSuaPhieu
                    {
                        MaPhieu = pn.MaPN,
                        LoaiPhieu = "Nhap",
                        TruongSua = "Tạo mới phiếu nhập",
                        GiaTriCu = "",
                        GiaTriMoi = $"Ngày tạo: {pn.NgayNhap:yyyy-MM-dd}",
                        ThoiGian = DateTime.Now,
                        NguoiSua = "Admin"
                    });
                    _context.SaveChanges();
                    // ===============================================================

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

            // 🔥 Lấy lịch sử sửa phiếu
            ViewBag.LichSu = _context.LichSuSuaPhieu
                .Where(x => x.MaPhieu == id && x.LoaiPhieu == "Nhap")
                .OrderByDescending(x => x.ThoiGian)
                .ToList();

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
                    var old = _context.PhieuNhap.FirstOrDefault(x => x.MaPN == pn.MaPN);

                    if (old == null)
                    {
                        ModelState.AddModelError("", "Phiếu không tồn tại!");
                        return View(pn);
                    }

                    // ==========================
                    // Ghi lịch sử
                    // ==========================
                    if (old.NgayNhap != pn.NgayNhap)
                        SaveHistory(old.MaPN, "Ngày nhập", old.NgayNhap.ToString("yyyy-MM-dd"), pn.NgayNhap.ToString("yyyy-MM-dd"));

                    if (old.MaNV != pn.MaNV)
                        SaveHistory(old.MaPN, "Nhân viên", old.MaNV, pn.MaNV);

                    if (old.MaNCC != pn.MaNCC)
                        SaveHistory(old.MaPN, "Nhà cung cấp", old.MaNCC, pn.MaNCC);

                    // ==========================
                    // Cập nhật dữ liệu
                    // ==========================
                    old.NgayNhap = pn.NgayNhap;
                    old.MaNV = pn.MaNV;
                    old.MaNCC = pn.MaNCC;

                    _context.SaveChanges();

                    return RedirectToAction("Details", new { id = pn.MaPN });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("",
                        "Lỗi cập nhật: " + ex.Message +
                        (ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "")
                    );
                }
            }

            ViewBag.NhanVien = _context.NhanVien.ToList();
            ViewBag.NCC = _context.NhaCungCap.ToList();
            return View(pn);
        }

        private void SaveHistory(string id, string field, string oldVal, string newVal)
        {
            _context.LichSuSuaPhieu.Add(new LichSuSuaPhieu
            {
                MaPhieu = id,
                LoaiPhieu = "Nhap",
                TruongSua = field,
                GiaTriCu = oldVal,
                GiaTriMoi = newVal,
                ThoiGian = DateTime.Now,
                NguoiSua = "Admin"
            });
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

            // 🔥 Lưu lịch sử xóa phiếu
            _context.LichSuSuaPhieu.Add(new LichSuSuaPhieu
            {
                MaPhieu = pn.MaPN,
                LoaiPhieu = "Nhap",
                TruongSua = "Xóa phiếu",
                GiaTriCu = "Phiếu còn tồn tại",
                GiaTriMoi = "Đã bị xóa",
                ThoiGian = DateTime.Now,
                NguoiSua = "Admin"
            });

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
