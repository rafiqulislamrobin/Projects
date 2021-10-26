using DataImporter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Entities
{
    public class Contact : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime ContactDate { get; set; }
        public int ExcelRow { get; set; }
        public int SequenceId { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
  
    }
}
