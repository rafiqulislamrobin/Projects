using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO = DataImporter.Info.Business_Object;
using EO = DataImporter.Info.Entities;
namespace DataImporter.Info.Profiles
{
    class DataImporterProfile : Profile
    {
        public DataImporterProfile()
        {
            CreateMap<EO.FilePath, BO.FilePath>().ReverseMap();
            CreateMap<EO.Group, BO.Group>().ReverseMap();
            CreateMap<EO.Contact, BO.Contact>().ReverseMap();
        }
    }
}
