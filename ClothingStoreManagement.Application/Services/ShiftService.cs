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
        /// هنا عشان ال Session 
        public async Task<Result<ShiftDTO>> GetOpenShiftAsync()
        {
            var shift = await _db.Shifts.GetAll().Include(s => s.User)  
                .OrderByDescending(s => s.Id).FirstOrDefaultAsync(s => s.EndTime == null);
            if (shift == null) 
                return Result<ShiftDTO>.Failure("No open shift found.", ErrorType.notFound);

            var payments = await _db.InvoicePayments.GetAll()
                .Where(p => p.Invoice.ShiftId == shift.Id)
                .GroupBy(p => new { p.PaymentSource.Name, p.PaymentSource.IsCashSource })
                .Select(g => new PaymentTypeSummary
                {
                    Name = g.Key.Name,
                    IsCashSource = g.Key.IsCashSource,
                    TotalAmount = g.Sum(p => p.Amount)
                })
                .ToListAsync();
            // 
            return Result<ShiftDTO>.Success(new ShiftDTO
            {
                Id = shift.Id,
                InitialCash = shift.InitialCash,
                StartTime = shift.StartTime,
                OpenBy = shift.User.UserName,
    
            });
        }
    }
}
