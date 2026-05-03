using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Application.DTO
{
    public class ProductListDto
    {
        public Guid Id { get; set; }    
        public string Name { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string CategoryName { get; set; } = null!; 
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }  
        public DateTime LastUpdate { get; set; }    

        public IEnumerable<ProductVariantListDto> Variants { get; set; } = new List<ProductVariantListDto>();
    }
}
