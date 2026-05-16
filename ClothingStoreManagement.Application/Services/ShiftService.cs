using AutoMapper;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Application.Services
{
    public class ShiftService
    {
        private readonly IUnitOfWork _db;
        private readonly AppState _appState;
        public ShiftService(IUnitOfWork db, AppState appState)
        {
            _db = db;
            _appState = appState;
        }
        public async Task<Result<ShiftDTO>> CreateShiftAsync(CreateShiftDTO dto)
        {
            var shift = new Shift(dto.InitialCash, _appState.CurrentUser!.Id);
            await _db.Shifts.CreateAsync(shift);
            await _db.ShiftTransactions.CreateAsync
                (new ShiftTransaction(_appState.CurrentUser!.Id,
                shift.Id, dto.InitialCash, TransactionType.OpeningBalance, "فتح الوردية  ")
                {
                    Shift = shift,  
                }) ; 
            await _db.Save();
            _db.Clear();
            return Result<ShiftDTO>.Success(new  ShiftDTO
            {
                Id = shift.Id,  
                InitialCash = dto.InitialCash,
                StartTime   = shift.StartTime,  
                OpenBy = _appState.CurrentUser!.UserName    , 
 
            });   
        }
        public async Task<Result<ShiftDTO>> GetOpenShiftAsync()
        {
            var shift = await _db.Shifts.GetAll().Include(s => s.User)  
                .OrderByDescending(s => s.Id).FirstOrDefaultAsync(s => s.EndTime == null);
            if (shift == null) 
                return Result<ShiftDTO>.Failure("No open shift found.", ErrorType.notFound);

            var cashPayments = await _db.InvoicePayments.GetAll()
         .Where(p => p.Invoice.ShiftId == shift.Id &&
                     p.PaymentSource.IsCashSource &&
                     p.Invoice.Status == InvoiceStatus.completed)
         .GroupBy(p => p.PaymentSource.Name)
         .Select(g => new PaymentTypeSummary
         {
             Name = g.Key,
             IsCashSource = true,
             TotalAmount = g.Sum(p => p.Amount)
         })
         .ToListAsync();

            var nonCashPayments = await _db.InvoicePayments.GetAll()
                .Where(p => p.Invoice.ShiftId == shift.Id &&
                            !p.PaymentSource.IsCashSource)
                .GroupBy(p => p.PaymentSource.Name)
                .Select(g => new PaymentTypeSummary
                {
                    Name = g.Key,
                    IsCashSource = false,
                    TotalAmount = g.Sum(p => p.Amount)
                })
                .ToListAsync();

            var allPaymentsSummary = cashPayments.Concat(nonCashPayments).ToList(); 
            var expand = await _db.ShiftTransactions.GetAll()
                .Where(st => st.ShiftId == shift.Id && (st.Type == TransactionType.Return
                || st.Type == TransactionType.Expense)).SumAsync(st => st.Amount);

            var adjustments = await _db.ShiftTransactions.GetAll()
                .Where(st => st.ShiftId == shift.Id && st.Type == TransactionType.Adjustment).SumAsync(st => st.Amount);
            return Result<ShiftDTO>.Success(new ShiftDTO
            {
                Id = shift.Id,
                InitialCash = shift.InitialCash,
                StartTime = shift.StartTime,
                OpenBy = shift.User.UserName,
                PaymentMethods = allPaymentsSummary,
                TotalExpenses = expand, 
                Adjustments = adjustments   
            }); 
        }
        public async Task <decimal> GetTotalExpectedCash (int shiftId )
        {
            var initial = _db.Shifts.GetAll().Where(s => s.Id == shiftId).Select(s => s.InitialCash).FirstOrDefault(); 
            var payments = await _db.InvoicePayments.GetAll()
            .Where(p => p.Invoice.ShiftId == shiftId && p.Invoice.Status == InvoiceStatus.completed)
            .GroupBy(p => new { p.PaymentSource.Name, p.PaymentSource.IsCashSource })
            .Select(g => new PaymentTypeSummary
            {
                Name = g.Key.Name,
                IsCashSource = g.Key.IsCashSource,
                TotalAmount = g.Sum(p => p.Amount)
            })
            .ToListAsync();
            var expand = await _db.ShiftTransactions.GetAll()
                .Where(st => st.ShiftId == shiftId && (st.Type == TransactionType.Return
                || st.Type == TransactionType.Expense)).SumAsync(st => st.Amount);

            var adjustments = await _db.ShiftTransactions.GetAll()
                .Where(st => st.ShiftId == shiftId && st.Type == TransactionType.Adjustment).SumAsync(st => st.Amount);
           
            var cashSales = payments.FirstOrDefault(p => p.IsCashSource)?.TotalAmount ?? 0;

            return initial + cashSales + adjustments - Math.Abs(expand);
        }
        public async Task<Result<string>> AddTransaction (int  shiftId ,  TransactionDTO  dto )
        {
            var shift = await _db.Shifts.FirstOrDefaultAsync(s => s.Id == shiftId);
            if (shift == null)
                return Result<string>.Failure("هذه الوردية غير موجودة " , ErrorType.notFound); 
                await   _db.ShiftTransactions.CreateAsync
                (new ShiftTransaction(_appState.CurrentUser!.Id,
               shiftId, dto.Amount, dto.Type, dto.Description));
            await _db.Save(); 

            return Result<string>.Success(); 
        }
        public async Task <Result <IEnumerable<TransactionListDTO>> > GetAllShfitTransactions (int shiftId)
        {
            var shift = await _db.Shifts.FirstOrDefaultAsync(s => s.Id == shiftId);
            if (shift == null)
                return Result < IEnumerable<TransactionListDTO>>.Failure("هذه الوردية غير موجودة ", ErrorType.notFound);
            var transactions = await _db.ShiftTransactions.GetAll().Where(st => st.ShiftId == shiftId).OrderByDescending(st=> st.CreatedAt).Select(st =>  new TransactionListDTO
            {
                Amount = st.Amount ,
                Description = st.Description    , 
                Type = st.Type , 
                CreatedAt = st.CreatedAt , 
                CreatedBy = st.User.UserName
            }).ToListAsync();
            return Result<IEnumerable<TransactionListDTO>>.Success(transactions); 
        }

        public async Task <Result<string>> CloseShift (int shiftId , decimal actualCash )
        {
            var shift = await _db.Shifts.FirstOrDefaultAsync (s => s.Id == shiftId , true) ;
            if (shift == null || shift.IsActive == false)
                return Result<string>.Failure("هذه الوردية غير موجودة او غير صالحة  ", ErrorType.notFound);
            var cashSales = await _db.InvoicePayments.GetAll()
                        .Where(p => p.Invoice.ShiftId == shift.Id && p.PaymentSource.IsCashSource && p.Invoice.Status == InvoiceStatus.completed).SumAsync(p => p.Amount); 
    
            var nonCashSales = await _db.InvoicePayments.GetAll()
                        .Where(p => p.Invoice.ShiftId == shift.Id && !p.PaymentSource.IsCashSource ).SumAsync(p => p.Amount);
          
            var transactionsSummary = await _db.ShiftTransactions.GetAll()
         .Where(st => st.ShiftId == shift.Id)
         .GroupBy(st => st.Type)
         .Select(g => new {
             Type = g.Key,
             Total = g.Sum(x => x.Amount)
         })
         .ToListAsync();

            var totalReturns = Math.Abs(transactionsSummary.FirstOrDefault(t => t.Type == TransactionType.Return)?.Total ?? 0);
            var totalExpenses = Math.Abs(transactionsSummary.FirstOrDefault(t => t.Type == TransactionType.Expense)?.Total ?? 0);
            var adjustments = transactionsSummary.FirstOrDefault(t => t.Type == TransactionType.Adjustment)?.Total ?? 0; ;

            shift.CloseShift(actualCash, cashSales, nonCashSales, totalReturns, totalExpenses, adjustments, _appState.CurrentUser.Id);
            string statusWord = shift.Difference < 0 ? "بعجز " : (shift.Difference > 0 ? "بزيادة " : "منتظمة ");

            string note = $"إغلاق وترحيل نقدية الوردية {statusWord}" +
                          (shift.Difference != 0 ? $"({Math.Abs(shift.Difference)} ج.م)" : "");
            await _db.TreasuryTransactions
                .CreateAsync(new
                MainTreasuryTransaction(shift.FinalCashInDrawer, TreasuryTransactionType.ShiftTransfer, note, shiftId)); 

            await _db.Save();
            _appState.SetCurrentShift(null!);
           return  Result<string>.Success();
        }
    }

}
