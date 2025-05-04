using System.ComponentModel.DataAnnotations;

namespace Tea.Application.DTOs.Items
{
    public class ItemExcelImportDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được trống")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug không được trống")]
        public string Slug { get; set; } = string.Empty;


        [Required(ErrorMessage = "ImgUrl không được trống")]
        public string ImgUrl { get; set; } = string.Empty;

        public string PublicId { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public bool IsPublished { get; set; } = true;

        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "ImgUrl không được trống")]
        public string CategoryList { get; set; } = string.Empty;


        [Required(ErrorMessage = "Tên size không được trống")]
        public string SizeName { get; set; } = string.Empty;


        public string SizeDescription { get; set; } = string.Empty;

        public decimal SizePrice { get; set; }

        public decimal? SizeNewPrice { get; set; }

        public bool SizeIsDeleted { get; set; }
    }
}
