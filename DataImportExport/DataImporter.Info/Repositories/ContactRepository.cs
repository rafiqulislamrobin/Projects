using DataImporter.Data;
using DataImporter.Info.Context;
using DataImporter.Info.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Repositories
{
    public class ContactRepository : Repository<Contact, Guid>, IContactRepository
    {
        public ContactRepository(IDataImporterDbContext context)
        : base((DbContext)context)
        {

        }
    }
}
