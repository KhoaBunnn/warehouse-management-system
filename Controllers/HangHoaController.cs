using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLKhoHang.Models;
using QLKhoHang.Models.ViewModels;
using QLKhoHang.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace QLKhoHang.Controllers
{
    [Authorize(Roles = "Admin,QuanLy,NhanVienKho")]
    public class HangHoaController : Controller
    {
        private readonly IHangHoaRepository _hangRepo;
        private readonly IKhoRepository _khoRepo;
        private readonly ILoaiHangRepository _loaiRepo;

        public HangHoaController(IHangHoaRepository hangRepo, IKhoRepository khoRepo, ILoaiHangRepository loaiRepo)
        {
            _hangRepo = hangRepo;
            _khoRepo = khoRepo;
            _loaiRepo = loaiRepo;
        }

        private async Task LoadDropdowns(HangHoaBaseViewModel model)
        {
            model.KhoList = (await _khoRepo.GetAllAsync()).Select(k => new SelectListItem
            {
                Value = k.MaKho,
                Text = k.TenKho,
                Selected = model.MaKho == k.MaKho
            });

            model.LoaiHangList = (await _loaiRepo.GetAllAsync()).Select(l => new SelectListItem
            {
                Value = l.MaLoai,
                Text = l.TenLoai,
                Selected = model.MaLoai == l.MaLoai
            });
        }

        // GET: HangHoa
        // GET: HangHoa
        // GET: HangHoa
        public async Task<IActionResult> Index(string q)
        {
            var list = await _hangRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(q))
            {
                list = list.Where(h =>
                    h.MaHang.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    h.TenHang.Contains(q, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            ViewBag.SearchString = q;
            return View(list);
        }


        // GET: HangHoa/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var hang = await _hangRepo.GetByIdAsync(id);
            if (hang == null) return NotFound();

            return View(hang);
        }

        // GET: HangHoa/Create
        public async Task<IActionResult> Create()
        {
            var model = new HangHoaCreateViewModel();

            // Generate Mã Hàng
            string prefix = "MH";
            int next = 1;

            var hangs = await _hangRepo.GetAllAsync();
            var has = hangs.Where(h => h.MaHang.StartsWith(prefix));

            if (has.Any())
            {
                var maxCode = has.Max(h => h.MaHang);
                if (int.TryParse(maxCode.Substring(2), out int num))
                {
                    next = num + 1;
                }
            }

            model.MaHang = prefix + next.ToString("D4");
            await LoadDropdowns(model);

            return View(model);
        }

        // POST: HangHoa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HangHoaCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // IN RA TOÀN BỘ LỖI MODELSTATE
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"[MODELSTATE ERROR] {entry.Key} => {error.ErrorMessage}");
                    }
                }

                await LoadDropdowns(model);
                return View(model);
            }

            var entity = new HangHoa
            {
                MaHang = model.MaHang,
                TenHang = model.TenHang,
                DonViTinh = model.DonViTinh,
                SoLuongTon = model.SoLuongTon,
                GiaNhap = model.GiaNhap,
                GiaXuat = model.GiaXuat,
                MaKho = model.MaKho,
                MaLoai = model.MaLoai
            };

            await _hangRepo.AddAsync(entity);
            await _hangRepo.SaveChangesAsync();

            TempData["Success"] = "Thêm hàng hóa thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: HangHoa/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var hang = await _hangRepo.GetByIdAsync(id);
            if (hang == null) return NotFound();

            var model = new HangHoaEditViewModel
            {
                MaHang = hang.MaHang,
                TenHang = hang.TenHang,
                DonViTinh = hang.DonViTinh,
                SoLuongTon = hang.SoLuongTon,
                GiaNhap = hang.GiaNhap,
                GiaXuat = hang.GiaXuat,
                MaKho = hang.MaKho,
                MaLoai = hang.MaLoai
            };

            await LoadDropdowns(model);

            return View(model);
        }

        // POST: HangHoa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HangHoaEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // IN RA TOÀN BỘ LỖI MODELSTATE
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"[MODELSTATE ERROR] {entry.Key} => {error.ErrorMessage}");
                    }
                }

                await LoadDropdowns(model);
                return View(model);
            }

            var hang = await _hangRepo.GetByIdAsync(model.MaHang);
            if (hang == null) return NotFound();

            hang.TenHang = model.TenHang;
            hang.DonViTinh = model.DonViTinh;
            hang.SoLuongTon = model.SoLuongTon;
            hang.GiaNhap = model.GiaNhap;
            hang.GiaXuat = model.GiaXuat;
            hang.MaLoai = model.MaLoai;
            hang.MaKho = model.MaKho;

            _hangRepo.Update(hang);
            await _hangRepo.SaveChangesAsync();

            TempData["Success"] = "Cập nhật hàng hóa thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: HangHoa/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var hang = await _hangRepo.GetByIdAsync(id);
            if (hang == null) return NotFound();

            return View(hang);
        }

        // POST: HangHoa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hang = await _hangRepo.GetByIdAsync(id);
            if (hang == null) return NotFound();

            _hangRepo.Delete(hang);
            await _hangRepo.SaveChangesAsync();

            TempData["Success"] = "Xóa hàng hóa thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
