using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace ClothingStoreManagement.Application.Services
{
    public class InvoiceService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        public InvoiceService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<InvoiceDTO>> CreateInvoiceAsync()
        {
            var invoice = new Invoice();
            await _db.Invoices.CreateAsync(invoice);
            await _db.Save();
            return Result<InvoiceDTO>.Success(new InvoiceDTO
            {
                Id = invoice.Id,
                Status = invoice.Status , 
                SerialNumber = invoice.Serial,  
                LastUpdate = invoice.LastUpdatedAt == null ? invoice.CreatedAt : invoice.LastUpdatedAt.Value, 
                TotalAmount = invoice.TotalAmount
            });
        }
    
    }
}


