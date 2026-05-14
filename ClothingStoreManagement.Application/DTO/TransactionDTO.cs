using ClothingStoreManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class TransactionDTO
    {
        [Range (1 , (double) decimal.MaxValue)]
        public decimal Amount { get; set; }
        [Required (ErrorMessage = "ادخل سبب العملية ")]
        public string Description { get; set; } 
        public TransactionType Type { get; set; }
        public string AdjustmentDirection { get; set; } = "in"; // "in" or "out"
    }
    public class TransactionListDTO
    {
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "ادخل سبب العملية ")]
        public string Description { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt {  get; set; }
        public string CreatedBy { get; set; }   
    }
}
