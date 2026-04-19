namespace ClothingStoreManagement.Application.DTO
{
    public class CategoryListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ProductsCount { get; set; } 
    }
}
