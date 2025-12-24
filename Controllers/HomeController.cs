using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QLKhoHang.Data;
using QLKhoHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace QLKhoHang.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QLKhoHangContext _context;

        public HomeController(ILogger<HomeController> logger, QLKhoHangContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeDashboardViewModel();

            vm.TotalProducts = await _context.HangHoa.CountAsync();
            vm.TotalPhieuNhap = await _context.PhieuNhap.CountAsync();
            vm.TotalPhieuXuat = await _context.PhieuXuat.CountAsync();

            var today = DateTime.Today;
            var todayStart = today;
            var todayEnd = today.AddDays(1);

            vm.TodayRevenue = await _context.CT_PhieuXuat
                .Include(ct => ct.PhieuXuat)
                .Where(ct => ct.PhieuXuat.NgayXuat >= todayStart && ct.PhieuXuat.NgayXuat < todayEnd)
                .SumAsync(ct => (decimal?)ct.SoLuong * ct.DonGiaXuat) ?? 0m;

            // Last 7 days labels and series (sum of quantities) - use transaction quantities for meaningful charts
            for (int i = 6; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);
                vm.ChartLabels.Add(day.ToString("MM-dd"));

                var dayStart = day;
                var dayEnd = day.AddDays(1);

                var nip = await _context.CT_PhieuNhap
                    .Include(ct => ct.PhieuNhap)
                    .Where(ct => ct.PhieuNhap.NgayNhap >= dayStart && ct.PhieuNhap.NgayNhap < dayEnd)
                    .SumAsync(ct => (int?)ct.SoLuong) ?? 0;

                var xip = await _context.CT_PhieuXuat
                    .Include(ct => ct.PhieuXuat)
                    .Where(ct => ct.PhieuXuat.NgayXuat >= dayStart && ct.PhieuXuat.NgayXuat < dayEnd)
                    .SumAsync(ct => (int?)ct.SoLuong) ?? 0;

                vm.ChartNhapCounts.Add((int)nip);
                vm.ChartXuatCounts.Add((int)xip);
            }

            // Recent transactions: last 6 combined PhieuNhap and PhieuXuat ordered by date
            // fetch recent CT_PhieuNhap and CT_PhieuXuat as anonymous objects, then map to tuples in-memory
            var recentNhapAnon = await _context.CT_PhieuNhap
                .Include(ct => ct.PhieuNhap)
                .OrderByDescending(ct => ct.PhieuNhap.NgayNhap)
                .Take(6)
                .Select(ct => new { Id = ct.MaPN, Qty = ct.SoLuong, Date = ct.PhieuNhap.NgayNhap, Type = "Nhập" })
                .ToListAsync();

            var recentXuatAnon = await _context.CT_PhieuXuat
                .Include(ct => ct.PhieuXuat)
                .OrderByDescending(ct => ct.PhieuXuat.NgayXuat)
                .Take(6)
                .Select(ct => new { Id = ct.MaPX, Qty = ct.SoLuong, Date = ct.PhieuXuat.NgayXuat, Type = "Xuất" })
                .ToListAsync();

            var merged = new List<(string Id, string Type, int Qty, DateTime Date)>();
            merged.AddRange(recentNhapAnon.Select(a => (a.Id, a.Type, a.Qty, a.Date)));
            merged.AddRange(recentXuatAnon.Select(a => (a.Id, a.Type, a.Qty, a.Date)));

            foreach (var t in merged.OrderByDescending(m => m.Date).Take(6))
            {
                vm.RecentTransactions.Add((t.Id, t.Type, t.Qty));
            }

            // Inventory value and low-stock
            vm.InventoryValue = await _context.HangHoa
                .Select(h => (decimal?)h.SoLuongTon * h.GiaNhap)
                .SumAsync() ?? 0m;

            // Low stock threshold (default 10) - show count of SKUs at or below threshold
            const int lowStockThreshold = 10;
            vm.LowStockCount = await _context.HangHoa.CountAsync(h => h.SoLuongTon <= lowStockThreshold);

            // Top selling products (last 30 days)
            var since = DateTime.Today.AddDays(-30);
            var topQuery = await _context.CT_PhieuXuat
                .Include(ct => ct.PhieuXuat)
                .Where(ct => ct.PhieuXuat.NgayXuat >= since)
                .GroupBy(ct => ct.MaHang)
                .Select(g => new { MaHang = g.Key, Qty = g.Sum(x => x.SoLuong) })
                .OrderByDescending(x => x.Qty)
                .Take(5)
                .ToListAsync();

            if (topQuery.Any())
            {
                var maList = topQuery.Select(t => t.MaHang).ToList();
                var names = await _context.HangHoa
                    .Where(h => maList.Contains(h.MaHang))
                    .ToDictionaryAsync(h => h.MaHang, h => h.TenHang);

                vm.TopProducts = topQuery.Select(t => new TopProductViewModel
                {
                    TenHang = names.ContainsKey(t.MaHang) ? names[t.MaHang] : t.MaHang,
                    SoLuongBan = t.Qty
                }).ToList();
            }

            // Stock by category
            var cat = await _context.HangHoa
                .Include(h => h.LoaiHang)
                .GroupBy(h => h.LoaiHang.TenLoai)
                .Select(g => new { TenLoai = g.Key ?? "(Không xác định)", Qty = g.Sum(x => x.SoLuongTon) })
                .OrderByDescending(x => x.Qty)
                .ToListAsync();

            vm.CategoryLabels = cat.Select(c => c.TenLoai).ToArray();
            vm.CategoryValues = cat.Select(c => (int)c.Qty).ToArray();

            // Today's quantities (for small daily stats)
            var todayStart2 = DateTime.Today;
            var todayEnd2 = todayStart2.AddDays(1);
            vm.TodayNhapQty = await _context.CT_PhieuNhap
                .Include(ct => ct.PhieuNhap)
                .Where(ct => ct.PhieuNhap.NgayNhap >= todayStart2 && ct.PhieuNhap.NgayNhap < todayEnd2)
                .SumAsync(ct => (int?)ct.SoLuong) ?? 0;

            vm.TodayXuatQty = await _context.CT_PhieuXuat
                .Include(ct => ct.PhieuXuat)
                .Where(ct => ct.PhieuXuat.NgayXuat >= todayStart2 && ct.PhieuXuat.NgayXuat < todayEnd2)
                .SumAsync(ct => (int?)ct.SoLuong) ?? 0;
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
