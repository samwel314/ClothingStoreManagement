using ClothingStoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    internal class ColorRepository : BaseRepository<Color> ,  IColorRepository
    {
        private readonly ApplicationDbContext _db;

        public ColorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
