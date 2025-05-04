using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("LienHe")]
    public class LienHe
    {
        [Key]
        public int malienhe { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string hoten { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string dienthoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        [StringLength(2000)]
        [Display(Name = "Nội dung")]
        public string noidung { get; set; }

        [Display(Name = "Ngày gửi")]
        public DateTime? ngaygui { get; set; }

        [Display(Name = "Trạng thái")]
        public int? trangthai { get; set; } // 0: Chưa liên hệ, 1: Đã liên hệ, 2: Đã hủy
    }
} 