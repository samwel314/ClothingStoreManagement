using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ClothingStoreManagement.Domain.Entities
{
    public class Color
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;
        public void Update(string name, string code)
        {
            Validate(name, code);
            Name = name.Trim();
            Code = code.Trim().ToUpper();
        }
        void Validate(string name, string code)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required");
        }

        private Color() { } // EF Core
        public Color(string name, string code)
        {
            Validate(name, code);
            Name = name.Trim();
            Code = code.Trim().ToUpper();
        }
    }
}
