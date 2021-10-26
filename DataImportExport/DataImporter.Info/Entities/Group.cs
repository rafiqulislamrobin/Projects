using DataImporter.Data;
using DataImporter.MemberShip.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Entities
{
    public class Group : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<FilePath> FilePaths { get; set; }
        public List<ExportStatus> ExportStatusEntities { get; set; }

    }
}
