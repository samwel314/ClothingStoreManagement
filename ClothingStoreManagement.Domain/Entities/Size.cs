namespace ClothingStoreManagement.Domain.Entities
{
    public class Size
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!; // Large
        public string Code { get; private set; } = null!; // L
        private Size() { } // EF Core

        public Size(string name, string code)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required");
            Name = name;
            Code = code.ToUpper();
        }
    }
}
