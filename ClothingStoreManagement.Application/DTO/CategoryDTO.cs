using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreManagement.Application.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
    public class CreateShiftDTO
    {
        public decimal InitialCash { get; set; } 
    }
}
