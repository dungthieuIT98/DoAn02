using System;

namespace DoAn.Models
{
    public class ProductViewModel
    {
        public SanPham Product { get; set; }
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
    }
} 