using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Data.Repository
{
    public interface IUnitOfWork 
    { 
        IColorRepository Colors { get; }
        ISizeRepository Sizes { get; }  
        ICategoryRepository Categories { get; }  
        Task Save();
    }
    
}
