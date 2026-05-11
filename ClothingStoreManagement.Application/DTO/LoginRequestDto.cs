using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "يرجى إدخال اسم المستخدم")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى إدخال كلمة المرور")]
        public string Password { get; set; } = string.Empty;
    }
}
