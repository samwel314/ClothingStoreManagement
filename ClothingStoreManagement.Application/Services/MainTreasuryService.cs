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

        public async Task<decimal>
          GetMainTreasuryTotal()
        {
            var transactions = await _db.TreasuryTransactions.GetAll().SumAsync(t => t.Amount);

            return transactions; 
        }
        public async Task<Result<IEnumerable<DisplayTreasuryTransactionDTO>>> GetMainTreasuryTransactionsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _db.TreasuryTransactions.GetAll();

            if (fromDate.HasValue) query = query.Where(t => t.CreatedAt >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var transactions = await query.Select(t => new DisplayTreasuryTransactionDTO
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
        public async Task<IEnumerable<DisplayPaymentPartDto>> GetTotalForNonCash(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _db.InvoicePayments.GetAll().Where(p => p.PaymentSource.IsCashSource == false);

            if (fromDate.HasValue) 
                query = query.Where(p => p.CreatedAt >= fromDate.Value.Date);
            if (toDate.HasValue) 
                query = query.Where(p => p.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var nonCashPayments = await query
              .GroupBy(p => new { p.PaymentSource.Name, p.PaymentSource.Id })
              .Select(g => new DisplayPaymentPartDto
              {
                  Name = g.Key.Name,
                  Amount = g.Sum(p => p.Amount),
                  PaymentSourceId = g.Key.Id,
              }).ToListAsync();

            return nonCashPayments;
        }
        public async Task<decimal> GetTotalStock()
        {
            var totalStock = await _db.ProductVariants.GetAll()
              .SumAsync(pv => pv.StockQuantity * pv.PurchasePrice);
            return totalStock;
        }
        public async Task<decimal> GetTotalExpand(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var shiftQuery = _db.ShiftTransactions.GetAll().Where(t => t.Type == TransactionType.Expense);
            var treasuryQuery = _db.TreasuryTransactions.GetAll().Where(t => t.Type == TreasuryTransactionType.SupplierPayment || t.Type == TreasuryTransactionType.GeneralExpense);

            if (fromDate.HasValue)
            {
                shiftQuery = shiftQuery.Where(t => t.CreatedAt >= fromDate.Value.Date);
                treasuryQuery = treasuryQuery.Where(t => t.CreatedAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                shiftQuery = shiftQuery.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
                treasuryQuery = treasuryQuery.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var expandStock = await shiftQuery.SumAsync(t => t.Amount);
            expandStock += await treasuryQuery.SumAsync(t => t.Amount);

            return Math.Abs(expandStock);
        }
        public async Task<decimal> GetTotalAdjustment(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var shiftQuery = _db.ShiftTransactions.GetAll().Where(t => t.Type == TransactionType.Adjustment);
            var treasuryQuery = _db.TreasuryTransactions.GetAll().Where(t => t.Type  == TreasuryTransactionType.ManualAdjustment);

            if (fromDate.HasValue)
            {
                shiftQuery = shiftQuery.Where(t => t.CreatedAt >= fromDate.Value.Date);
                treasuryQuery = treasuryQuery.Where(t => t.CreatedAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                shiftQuery = shiftQuery.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
                treasuryQuery = treasuryQuery.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var adjustments = await shiftQuery.SumAsync(t => t.Amount);
            adjustments += await treasuryQuery.SumAsync(t => t.Amount);

            return adjustments;
        }
        public async Task<decimal> GetOwnerExpand(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _db.TreasuryTransactions.GetAll().Where(t => t.Type == TreasuryTransactionType.OwnerWithdrawal);

            if (fromDate.HasValue) query = query.Where(t => t.CreatedAt >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var OwnerExpand = await query.SumAsync(t => t.Amount);
            return Math.Abs(OwnerExpand);
        }
        public async Task<decimal> GetProfit(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _db.Invoices.GetAll().Where(i => i.Status == InvoiceStatus.completed);

            if (fromDate.HasValue) 
                query = query.Where(i => i.CreatedAt >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(i => i.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var NetProfit = await query
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


        public async Task<IEnumerable<SystemTransactionDTO>> ExpandTransaction(DateTime? fromDate =  null, DateTime? toDate = null)
        {
            var shiftQuery =
                _db.ShiftTransactions.GetAll()
                .Where(t => t.Type == TransactionType.Expense);
            var treasuryQuery = _db.TreasuryTransactions.GetAll().
             Where(t => t.Type == TreasuryTransactionType.SupplierPayment ||
             t.Type == TreasuryTransactionType.GeneralExpense); 
            if (fromDate.HasValue)
            {
                shiftQuery = shiftQuery.Where(t => t.CreatedAt >= fromDate.Value.Date);
                treasuryQuery = treasuryQuery.Where(t => t.CreatedAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                shiftQuery = shiftQuery.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
                treasuryQuery = treasuryQuery.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var shiftTransactions = shiftQuery.Select(st => new SystemTransactionDTO()
                {
                    Amount = st.Amount,
                    CreatedAt = st.CreatedAt,
                    By = st.User.UserName,
                    Note = st.Description,
                });
            var treasuryTransactions =  treasuryQuery.Select(t => new SystemTransactionDTO()
             {
                 Amount = t.Amount,
                 CreatedAt = t.CreatedAt,
                 By = "ادراي ",
                Note = t.Notes 
             });

            var allExpenses = await shiftTransactions.Union(treasuryTransactions)
                                              .OrderByDescending(t => t.CreatedAt)
                                              .ToListAsync();
            return allExpenses;
        }
        public async Task<IEnumerable<SystemTransactionDTO>> AdjustmentTransaction(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var shiftQuery =
                _db.ShiftTransactions.GetAll()
                .Where(t => t.Type == TransactionType.Adjustment);
            var treasuryQuery = _db.TreasuryTransactions.GetAll().
             Where(t => t.Type == TreasuryTransactionType.ManualAdjustment );
            if (fromDate.HasValue)
            {
                shiftQuery = shiftQuery.Where(t => t.CreatedAt >= fromDate.Value.Date);
                treasuryQuery = treasuryQuery.Where(t => t.CreatedAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                shiftQuery = shiftQuery.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
                treasuryQuery = treasuryQuery.Where(t => t.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var shiftTransactions = shiftQuery.Select(st => new SystemTransactionDTO()
            {
                Amount = st.Amount,
                CreatedAt = st.CreatedAt,
                By = st.User.UserName,
                Note = st.Description,
            });
            var treasuryTransactions = treasuryQuery.Select(t => new SystemTransactionDTO()
            {
                Amount = t.Amount,
                CreatedAt = t.CreatedAt,
                By = "ادراي ",
                Note = t.Notes
            });

            var allAdjustment = await shiftTransactions.Union(treasuryTransactions)
                                              .OrderByDescending(t => t.CreatedAt)
                                              .ToListAsync();
            return allAdjustment;
        }

        public async Task<IEnumerable<SystemTransactionDTO>> NonCashPay (int methodId , DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _db.InvoicePayments.GetAll().Where(p => p.PaymentSourceId == methodId);

            if (fromDate.HasValue)
                query = query.Where(p => p.CreatedAt >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(p => p.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));

           var methodTransaction =  await query.Select(ip => new SystemTransactionDTO()
           {
               Amount = ip.Amount,
               CreatedAt = ip.CreatedAt,
               By = ip.Invoice.User.UserName,
               Note  = $"بيع فاتورة {ip.Invoice.Serial} {(ip.Invoice.Status != InvoiceStatus.completed ? "المرتجعة" : "")} - ملاحظات: ({ip.Reference})",
           }).ToListAsync();

            return methodTransaction; 
        }


    }
}
