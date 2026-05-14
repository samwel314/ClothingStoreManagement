using ClothingStoreManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "اسم المستخدم يجب أن يكون (3-20 حرف)، إنجليزي أو أرقام فقط، وبدون مسافات.")]
        public string UserName { get; set; } 

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أرقام/حروف")]
        public string Password { get; set; }

        [Required(ErrorMessage = "الدور مطلوب")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "الدور غير صالح")]
        public UserRole Role { get; set; } = UserRole.Cashier;    
    }
    public class UpdateUserDto
    {
        public int Id { get; set; } 
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "اسم المستخدم يجب أن يكون (3-20 حرف)، إنجليزي أو أرقام فقط، وبدون مسافات.")]
        public string UserName { get; set; }
        [RegularExpression(@"^$|^.{6,}$", ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 حروف")]
        public string ? Password { get; set; }
        [EnumDataType(typeof(UserRole), ErrorMessage = "الدور غير صالح")]
        public UserRole Role { get; set; } = UserRole.Cashier;
    }
}
