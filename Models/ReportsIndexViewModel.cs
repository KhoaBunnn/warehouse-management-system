using System;
using System.Collections.Generic;

namespace QLKhoHang.Models
{
    public class ReportsIndexViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string MaHH { get; set; }
        public string MaKho { get; set; }

        // Dropdowns
        public IEnumerable<HangHoa> HangHoaList { get; set; }
        public IEnumerable<Kho> KhoList { get; set; }

        // Report data (dynamic rows)
        public IEnumerable<ReportEntryViewModel> NhapList { get; set; }
        public IEnumerable<ReportEntryViewModel> XuatList { get; set; }
        public IEnumerable<ReportEntryViewModel> TonList { get; set; }

        // Totals
        public int TotalNhapQty { get; set; }
        public decimal TotalNhapValue { get; set; }
        public int TotalXuatQty { get; set; }
        public decimal TotalXuatValue { get; set; }
        public int TotalTonQty { get; set; }
        public decimal TotalTonValue { get; set; }

        // Chart series
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<int> NhapSeries { get; set; } = new List<int>();
        public List<int> XuatSeries { get; set; } = new List<int>();

        // Stock by category (pie)
        public List<string> CategoryLabels { get; set; } = new List<string>();
        public List<int> CategoryValues { get; set; } = new List<int>();
    }
}
