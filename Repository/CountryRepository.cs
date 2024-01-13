using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class CountryRepository : RepositoryBase<Country>, ICountryRepository
    {
        private readonly DatabaseContext _context;

        public CountryRepository(DatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
