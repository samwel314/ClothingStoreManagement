using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Domain.Entities
{
    public enum TreasuryTransactionType
    {
        [Display(Name = "ترحيل وردية")]
        ShiftTransfer = 1,
        [Display(Name = "مصروفات عامة")]
        GeneralExpense = 2,
        [Display(Name = "دفع لمورد")]
        SupplierPayment = 3,
        [Display(Name = "مسحوبات المالك")]
        OwnerWithdrawal = 4,
        [Display(Name = "إيداع رأسمال")]
        CapitalInjection = 5,
        [Display(Name = "تسوية رصيد")]
        ManualAdjustment = 6
    }
}
