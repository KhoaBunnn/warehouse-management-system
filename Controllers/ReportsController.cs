using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhoHang.Data;
using QLKhoHang.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace QLKhoHang.Controllers
{
    public class ReportsController : Controller
    {
        private readonly QLKhoHangContext _context;

        public ReportsController(QLKhoHangContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, string maHH, string maKho)
        {
            var vm = new ReportsIndexViewModel();

            // prepare filter defaults
            vm.From = from ?? DateTime.Today.AddDays(-30);
            vm.To = to ?? DateTime.Today;
            vm.MaHH = maHH;
            vm.MaKho = maKho;

            // Dropdown lists
            vm.HangHoaList = await _context.HangHoa.OrderBy(h => h.TenHang).ToListAsync();
            vm.KhoList = await _context.Kho.OrderBy(k => k.TenKho).ToListAsync();

            // Build dynamic reports from CT_PhieuNhap / CT_PhieuXuat / HangHoa
            var nhapQuery = _context.CT_PhieuNhap
                .Include(ct => ct.PhieuNhap)
                .Include(ct => ct.HangHoa)
                .Where(ct => ct.PhieuNhap.NgayNhap.Date >= vm.From.Date && ct.PhieuNhap.NgayNhap.Date <= vm.To.Date);

            var xuatQuery = _context.CT_PhieuXuat
                .Include(ct => ct.PhieuXuat)
                .Include(ct => ct.HangHoa)
                .Where(ct => ct.PhieuXuat.NgayXuat.Date >= vm.From.Date && ct.PhieuXuat.NgayXuat.Date <= vm.To.Date);

            var tonQuery = _context.HangHoa.AsQueryable();

            if (!string.IsNullOrEmpty(vm.MaHH))
            {
                nhapQuery = nhapQuery.Where(ct => ct.MaHang == vm.MaHH);
                xuatQuery = xuatQuery.Where(ct => ct.MaHang == vm.MaHH);
                tonQuery = tonQuery.Where(h => h.MaHang == vm.MaHH);
            }
            if (!string.IsNullOrEmpty(vm.MaKho))
            {
                nhapQuery = nhapQuery.Where(ct => ct.HangHoa.MaKho == vm.MaKho);
                xuatQuery = xuatQuery.Where(ct => ct.HangHoa.MaKho == vm.MaKho);
                tonQuery = tonQuery.Where(h => h.MaKho == vm.MaKho);
            }

            var nhapList = await nhapQuery.ToListAsync();
            var xuatList = await xuatQuery.ToListAsync();
            var tonList = await tonQuery.ToListAsync();

            vm.NhapList = nhapList.Select(ct => new ReportEntryViewModel
            {
                Type = "Nhap",
                RefId = ct.MaPN,
                Ngay = ct.PhieuNhap.NgayNhap,
                MaHH = ct.MaHang,
                TenHang = ct.HangHoa?.TenHang,
                MaKho = ct.HangHoa?.MaKho,
                Quantity = ct.SoLuong,
                Value = ct.SoLuong * ct.DonGiaNhap
            }).ToList();

            vm.XuatList = xuatList.Select(ct => new ReportEntryViewModel
            {
                Type = "Xuat",
                RefId = ct.MaPX,
                Ngay = ct.PhieuXuat.NgayXuat,
                MaHH = ct.MaHang,
                TenHang = ct.HangHoa?.TenHang,
                MaKho = ct.HangHoa?.MaKho,
                Quantity = ct.SoLuong,
                Value = ct.SoLuong * ct.DonGiaXuat
            }).ToList();

            // For inventory (ton), use current HangHoa quantities as of report To date.
            vm.TonList = tonList.Select(h => new ReportEntryViewModel
            {
                Type = "Ton",
                RefId = h.MaHang,
                Ngay = vm.To,
                MaHH = h.MaHang,
                TenHang = h.TenHang,
                MaKho = h.MaKho,
                Quantity = h.SoLuongTon,
                Value = h.SoLuongTon * h.GiaNhap
            }).ToList();

            vm.TotalNhapQty = vm.NhapList.Sum(n => n.Quantity);
            vm.TotalNhapValue = vm.NhapList.Sum(n => n.Value);

            vm.TotalXuatQty = vm.XuatList.Sum(n => n.Quantity);
            vm.TotalXuatValue = vm.XuatList.Sum(n => n.Value);

            vm.TotalTonQty = vm.TonList.Sum(n => n.Quantity);
            vm.TotalTonValue = vm.TonList.Sum(n => n.Value);

            // Build chart labels (per-day) between From and To inclusive
            var days = (vm.To.Date - vm.From.Date).Days + 1;
            for (int i = 0; i < days; i++)
            {
                var day = vm.From.Date.AddDays(i);
                vm.ChartLabels.Add(day.ToString("MM-dd"));

                var nhapQty = nhapList.Where(ct => ct.PhieuNhap.NgayNhap.Date == day).Sum(ct => ct.SoLuong);
                var xuatQty = xuatList.Where(ct => ct.PhieuXuat.NgayXuat.Date == day).Sum(ct => ct.SoLuong);

                vm.NhapSeries.Add(nhapQty);
                vm.XuatSeries.Add(xuatQty);
            }

            // Category breakdown for current stock snapshot (group by MaLoai)
            var categoryGroup = tonList
                .GroupBy(h => h.MaLoai)
                .Select(g => new { MaLoai = g.Key, Qty = g.Sum(h => h.SoLuongTon) })
                .ToList();

            // Fetch category names
            var allLoai = await _context.LoaiHang.ToListAsync();
            foreach (var c in categoryGroup)
            {
                var name = allLoai.FirstOrDefault(l => l.MaLoai == c.MaLoai)?.TenLoai ?? c.MaLoai ?? "Khác";
                vm.CategoryLabels.Add(name);
                vm.CategoryValues.Add(c.Qty);
            }

            return View(vm);
        }

        // GET: Reports/ExportCsv
        public async Task<IActionResult> ExportCsv(DateTime? from, DateTime? to, string maHH, string maKho)
        {
            var vm = new ReportsIndexViewModel
            {
                From = from ?? DateTime.Today.AddDays(-30),
                To = to ?? DateTime.Today,
                MaHH = maHH,
                MaKho = maKho
            };

            // Build same dynamic queries as Index for export
            var nhapQuery = _context.CT_PhieuNhap
                .Include(ct => ct.PhieuNhap)
                .Include(ct => ct.HangHoa)
                .Where(ct => ct.PhieuNhap.NgayNhap.Date >= vm.From.Date && ct.PhieuNhap.NgayNhap.Date <= vm.To.Date);

            var xuatQuery = _context.CT_PhieuXuat
                .Include(ct => ct.PhieuXuat)
                .Include(ct => ct.HangHoa)
                .Where(ct => ct.PhieuXuat.NgayXuat.Date >= vm.From.Date && ct.PhieuXuat.NgayXuat.Date <= vm.To.Date);

            var tonQuery = _context.HangHoa.AsQueryable();

            if (!string.IsNullOrEmpty(vm.MaHH))
            {
                nhapQuery = nhapQuery.Where(ct => ct.MaHang == vm.MaHH);
                xuatQuery = xuatQuery.Where(ct => ct.MaHang == vm.MaHH);
                tonQuery = tonQuery.Where(h => h.MaHang == vm.MaHH);
            }
            if (!string.IsNullOrEmpty(vm.MaKho))
            {
                nhapQuery = nhapQuery.Where(ct => ct.HangHoa.MaKho == vm.MaKho);
                xuatQuery = xuatQuery.Where(ct => ct.HangHoa.MaKho == vm.MaKho);
                tonQuery = tonQuery.Where(h => h.MaKho == vm.MaKho);
            }


            // Prepare CSV from dynamic report entries
            using var mem = new MemoryStream();
            using var writer = new StreamWriter(mem, Encoding.UTF8, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write header
            csv.WriteField("Type");
            csv.WriteField("RefId");
            csv.WriteField("Ngay");
            csv.WriteField("MaHH");
            csv.WriteField("TenHang");
            csv.WriteField("MaKho");
            csv.WriteField("Quantity");
            csv.WriteField("Value");
            csv.NextRecord();

            var allRows = (await nhapQuery.ToListAsync()).Select(ct => new ReportEntryViewModel
            {
                Type = "Nhap",
                RefId = ct.MaPN,
                Ngay = ct.PhieuNhap.NgayNhap,
                MaHH = ct.MaHang,
                TenHang = ct.HangHoa?.TenHang,
                MaKho = ct.HangHoa?.MaKho,
                Quantity = ct.SoLuong,
                Value = ct.SoLuong * ct.DonGiaNhap
            }).Concat((await xuatQuery.ToListAsync()).Select(ct => new ReportEntryViewModel
            {
                Type = "Xuat",
                RefId = ct.MaPX,
                Ngay = ct.PhieuXuat.NgayXuat,
                MaHH = ct.MaHang,
                TenHang = ct.HangHoa?.TenHang,
                MaKho = ct.HangHoa?.MaKho,
                Quantity = ct.SoLuong,
                Value = ct.SoLuong * ct.DonGiaXuat
            })).Concat((await tonQuery.ToListAsync()).Select(h => new ReportEntryViewModel
            {
                Type = "Ton",
                RefId = h.MaHang,
                Ngay = vm.To,
                MaHH = h.MaHang,
                TenHang = h.TenHang,
                MaKho = h.MaKho,
                Quantity = h.SoLuongTon,
                Value = h.SoLuongTon * h.GiaNhap
            }));

            foreach (var r in allRows)
            {
                csv.WriteField(r.Type);
                csv.WriteField(r.RefId);
                csv.WriteField(r.Ngay.ToString("yyyy-MM-dd"));
                csv.WriteField(r.MaHH);
                csv.WriteField(r.TenHang);
                csv.WriteField(r.MaKho);
                csv.WriteField(r.Quantity);
                csv.WriteField(r.Value);
                csv.NextRecord();
            }

            await writer.FlushAsync();
            mem.Position = 0;

            var fileName = $"reports_{vm.From:yyyyMMdd}_{vm.To:yyyyMMdd}.csv";
            return File(mem.ToArray(), "text/csv; charset=utf-8", fileName);
        }
    }
}
