using ClothingStoreManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public UserRole RoleName { get; set; } 
        public bool IsActive { get; set; }
    }
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "يرجى إدخال اسم المستخدم")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى إدخال كلمة المرور")]
        public string Password { get; set; } = string.Empty;
    }

    public class UserSessionDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public UserRole Role { get; set; }
    }
}
