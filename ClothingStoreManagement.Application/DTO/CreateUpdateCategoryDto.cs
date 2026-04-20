using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateUpdateCategoryDto
    {
        public int ? Id { get; set; }   
        [MaxLength (100 , ErrorMessage = "اقصي عدد من الحروف هو 100 حرف ") ]
        [Required (ErrorMessage = "ادخل اسم الفئة")]
        public string Name { get; set; } = null!; 
        public int ? CategoryId { get; set; }   
    }
}
