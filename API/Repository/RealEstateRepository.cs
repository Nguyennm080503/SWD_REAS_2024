﻿using API.Data;
using API.Entity;
using API.Interfaces;

namespace API.Repository
{
    public class RealEstateRepository : BaseRepository<RealEstate>, IRealEstateRepository
    {
        private readonly DataContext _dataContext;
        public RealEstateRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }
    }
}