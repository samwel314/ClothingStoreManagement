using AutoMapper.QueryableExtensions;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Application.Services
{
    public class MainTreasuryService
    {
        private readonly IUnitOfWork _db;

        public MainTreasuryService(IUnitOfWork db)
        {
            _db = db;
        }

        public async Task<Result<IEnumerable<DisplayTreasuryTransactionDTO>>>
          GetMainTreasuryTransactionsAsync()
        {
            var transactions = await _db.TreasuryTransactions.GetAll().Select(t => new DisplayTreasuryTransactionDTO
            {
                Id = t.Id,
                Amount = t.Amount,
                ShiftId = t.ShiftId,
                Type = t.Type,
                Notes = t.Notes,
                By = t.Shift != null ? (t.Shift.User.UserName ?? "الإدارة") : "الإدارة (يدوي)",
                CreatedAt = t.CreatedAt,
            }).OrderByDescending(t => t.CreatedAt).ToListAsync();

            return Result<IEnumerable<DisplayTreasuryTransactionDTO>>.Success(transactions);
        }
        public async Task<IEnumerable<DisplayPaymentPartDto>> GetTotalForNonCash()
        {
            var nonCashPayments = await _db.InvoicePayments.GetAll()
              .Where(p => p.PaymentSource.IsCashSource == false)
              .GroupBy(p => new
              {
                  p.PaymentSource.Name,
                  p.PaymentSource.Id
              })
              .Select(g => new DisplayPaymentPartDto
              {
                  Name = g.Key.Name,
                  Amount = g.Sum(p => p.Amount),
                  PaymentSourceId = g.Key.Id,
              })
             .ToListAsync();
            return nonCashPayments;
        }
        public async Task<decimal> GetTotalStock()
        {
            var totalStock = await _db.ProductVariants.GetAll()
              .SumAsync(pv => pv.StockQuantity * pv.PurchasePrice);
            return totalStock;
        }
        public async Task<decimal> GetTotalExpand()
        {
            var expandStock = await _db.ShiftTransactions.GetAll().Where(t => t.Type == TransactionType.Expense)
              .SumAsync(t => t.Amount);
            expandStock += await _db.TreasuryTransactions.GetAll().
                Where(t => t.Type == TreasuryTransactionType.SupplierPayment
                || t.Type == TreasuryTransactionType.GeneralExpense)
              .SumAsync(t => t.Amount);

            return Math.Abs(expandStock);
        }
        public async Task<decimal> GetOwnerExpand()
        {
            var OwnerExpand = await _db.TreasuryTransactions.GetAll().
                Where(t => t.Type == TreasuryTransactionType.OwnerWithdrawal)
              .SumAsync(t => t.Amount);
            return Math.Abs(OwnerExpand); 
        }
        public async Task<decimal> GetProfit()
        {
            var NetProfit = await _db.Invoices.GetAll()
            .Where(i => i.Status == InvoiceStatus.completed)
            .SelectMany(i => i.Items)
            .SumAsync(ii => ((ii.SellingPrice * (1 - ii.Discount / 100)) - ii.PurchasePrice) * ii.Quantity);
            return NetProfit;
        }

        public async Task CreateTreasuryTransaction(TreasuryTransactionCreateDto dto  )
        {
            if (dto.Type == TreasuryTransactionType.GeneralExpense ||
                   dto.Type == TreasuryTransactionType.SupplierPayment ||
                   dto.Type == TreasuryTransactionType.OwnerWithdrawal ||
                   dto.Type == TreasuryTransactionType.CasherSupport ||
                   (dto.Type == TreasuryTransactionType.ManualAdjustment && dto.IsNegativeAdjustment)) 
            {
                            dto.Amount = -Math.Abs(dto.Amount);
            }
            else
            {
                dto.Amount = Math.Abs(dto.Amount);
            }

            await _db.TreasuryTransactions
               .CreateAsync(new
               MainTreasuryTransaction(dto.Amount, dto.Type!.Value , dto.Notes) );
           await _db.Save(); 
        }

    }
}
