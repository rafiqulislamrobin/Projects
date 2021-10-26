using DataImporter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Entities
{
    public class FilePath : IEntity<int>
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string FileName { get; set; }
        public string FilePathName { get; set; }
        public string GroupName { get; set; }
        public string FileStatus { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
