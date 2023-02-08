﻿using MagicVilla_API.Controllers;
using MagicVilla_API.Models;

namespace MagicVilla_API.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        public VillaNumberRepository(ApplicationDBContext db) : base(db)
        {
        }
    }
}