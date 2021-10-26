using DataImporter.Data;
using DataImporter.Info.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.UnitOfWorks
{ 
    public interface IDataUnitOfWork : IUnitOfWork
    {
        public IExportStatusRepository ExportStatus { get; }
        IFilePathRepository FilePath { get; }
        IGroupRepository Group { get; }
        IContactRepository Contact { get; }
    }
}
