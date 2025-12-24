using System;
using System.Collections.Generic;

namespace QLKhoHang.Models
{
    public class HomeDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalPhieuNhap { get; set; }
        public int TotalPhieuXuat { get; set; }
        public decimal TodayRevenue { get; set; }

        // Inventory specific metrics
        public decimal InventoryValue { get; set; }
        public int LowStockCount { get; set; }

        // Chart data
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<int> ChartNhapCounts { get; set; } = new List<int>();
        public List<int> ChartXuatCounts { get; set; } = new List<int>();

        // Additional metrics for redesigned dashboard
        public decimal MonthToDate { get; set; }
        public decimal AvgSales { get; set; }
        public List<int>? Pie1 { get; set; }
        public List<int>? Pie2 { get; set; }

        // Recent transactions (simple tuples: id, type, qty)
        public List<(string Id, string Type, int Qty)> RecentTransactions { get; set; } = new List<(string, string, int)>();

        public List<TopProductViewModel> TopProducts { get; set; } = new List<TopProductViewModel>();
        public string[] CategoryLabels { get; set; } = new string[0];
        public int[] CategoryValues { get; set; } = new int[0];
        // Thống kê theo ngày (hôm nay)
        public int TodayNhapQty { get; set; }
        public int TodayXuatQty { get; set; }
    }
}
