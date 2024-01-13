using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class LanguageRepository : RepositoryBase<Language>, ILanguageRepository
    {
        private readonly DatabaseContext _context;

        public LanguageRepository(DatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
