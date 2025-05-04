using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    public partial class DanhGiaSanPham1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDanhGia { get; set; }

        public int MaSanPham { get; set; }

        public int? MaNguoiDung { get; set; }

        [Range(1, 5)]
        public int DiemDanhGia { get; set; }

        public string NoiDungDanhGia { get; set; }

        public DateTime NgayDanhGia { get; set; }

        public string KhachHang { get; set; }
    }
} 