namespace ClothingStoreManagement.Domain.Entities
{
    public enum TransactionType
    {
        OpeningBalance = 1, // افتتاح الوردية (In)
        Sale = 2,           // بيع (In)
        Expense = 3,        // مصروف (Out)
        Return = 4,         // مرتجع (Out)
        Adjustment = 5      // تعديل يدوي من المدير
    }
}
