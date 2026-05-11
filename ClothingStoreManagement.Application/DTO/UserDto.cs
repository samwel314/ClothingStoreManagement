using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public UserRole RoleName { get; set; } 
        public bool IsActive { get; set; }
    }

    public class UserSessionDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public UserRole Role { get; set; }
    }
    public class ShiftSessionDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } 

    }
}
