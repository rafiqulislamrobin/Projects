using DataImporter.Data;
using DataImporter.Info.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Repositories
{
    public interface IExportStatusRepository : IRepository<ExportStatus, int>
    {
    }
}
