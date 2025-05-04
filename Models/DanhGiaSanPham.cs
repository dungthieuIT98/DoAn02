using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("DanhGiaSanPham")]
    public partial class DanhGiaSanPham
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

        public virtual SanPham SanPham { get; set; }
        public virtual KhachHang KhachHang { get; set; }
    }
} 