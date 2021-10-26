using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Business_Object
{
    public class Contact
    {
        public int Id { get; set; }
        public DateTime ContactDate { get; set; }
        public int GroupId { get; set; }
        public Dictionary<string, string> Properties = new();
         
    }
}
