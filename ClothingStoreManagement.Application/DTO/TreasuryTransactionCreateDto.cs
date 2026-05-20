using ClothingStoreManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class TreasuryTransactionCreateDto
    {
        [Required(ErrorMessage = "برجاء تحديد نوع المعاملة")]
        public TreasuryTransactionType? Type { get; set; }

        [Required(ErrorMessage = "برجاء إدخال المبلغ")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون المبلغ أكبر من صفر")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "برجاء كتابة السبب أو الملاحظة")]
        [StringLength(500, ErrorMessage = "الملاحظة طويلة جداً (الحد الأقصى 500 حرف)")]
        public string Notes { get; set; } = string.Empty;
        public bool IsNegativeAdjustment { get; set; } = false;
    }
}
