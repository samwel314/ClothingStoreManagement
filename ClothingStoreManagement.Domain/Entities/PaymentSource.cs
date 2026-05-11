namespace ClothingStoreManagement.Domain.Entities
{
    public class PaymentSource
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public bool IsActive { get; private set; }
        private PaymentSource() { }
        public PaymentSource(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("اسم مصدر الدفع مطلوب");

            Name = name;
            IsActive = true;
        }
        public void ToggleStatus() => IsActive = !IsActive;
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("الاسم الجديد غير صالح");
            Name = newName;
        }
    }
}
