using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Domain.Entities
{
    public enum TreasuryTransactionType
    {
        ShiftTransfer = 1,
        GeneralExpense = 2,
        SupplierPayment = 3,
        OwnerWithdrawal = 4,
        CapitalInjection = 5,
        ManualAdjustment = 6 ,
        CasherSupport  = 7
    }
}
