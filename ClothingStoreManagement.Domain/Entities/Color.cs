namespace ClothingStoreManagement.Domain.Entities
{
    public class Color
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!; 
        public string Code { get; private set; } = null!; 
        private Color() { } // EF Core
        public Color(string name, string code)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required");
            Name = name;
            Code = code.ToUpper();
        }
    }
}
