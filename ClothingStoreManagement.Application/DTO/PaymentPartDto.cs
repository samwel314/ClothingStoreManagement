using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class PaymentPartDto
    {
        public int PaymentSourceId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Amount { get; set; } = 0; // القيمة الافتراضية صفر
        public bool IsCash { get; set; }
        public bool IsVisible { get; set; }
        [Required (ErrorMessage = " ادخل مصدر الدفع (رقم المحفظة - رقم الريسيت - الخ ) ") ]
        public string Reference { get;  set; }

    }
}
