using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Data.Repository
{
    public interface IUnitOfWork 
    { 
        IColorRepository Colors { get; }
        ISizeRepository Sizes { get; }  
        Task Save();
    }
    
}
