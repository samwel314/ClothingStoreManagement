namespace ClothingStoreManagement.Domain.Entities
{
    public enum InvoiceStatus
    {
        pending,  //  مش بتخصم حاجو من المخزون 
        completed, // بتخصم 
        returned // بترجع 
                 // completed => returned 
                 // pending => completed
                 // returned مش هتتغير خلاص 
    }
}
