namespace ClothingStoreManagement.Domain.Entities
{
    public class Color
    {
        public int Id { get; private set; }

        public string Name { get; private set; } = null!;

        // Existing code - keep it to avoid breaking current code
        public string Code { get; private set; } = null!;

        public string HexCode { get; private set; } = null!;

        public void Update(string name, string code, string hexCode)
        {
            Validate(name, code, hexCode);

            Name = name.Trim();
            Code = code.Trim().ToUpper();
            HexCode = hexCode.Trim().ToUpper();
        }

        private void Validate(string name, string code, string hexCode)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code is required");

            if (string.IsNullOrWhiteSpace(hexCode))
                throw new ArgumentException("HexCode is required");
        }

        private Color() { } // EF Core

        public Color(string name, string code, string hexCode)
        {
            Validate(name, code, hexCode);

            Name = name.Trim();
            Code = code.Trim().ToUpper();
            HexCode = hexCode.Trim().ToUpper();
        }
    }
}
