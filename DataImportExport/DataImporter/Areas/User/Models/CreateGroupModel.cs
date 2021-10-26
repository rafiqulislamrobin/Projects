using Autofac;
using DataImporter.Info.Business_Object;
using DataImporter.Info.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataImporter.Areas.User.Models
{

    public class CreateGroupModel
    {

        public int Id { get; set; }
        [Required, MaxLength(100, ErrorMessage = "Nameshould be less than 100 characters")]
        public string Name { get; set; }

        private  IGroupServices _groupServices;
        private IHttpContextAccessor _httpContextAccessor;
        private ILifetimeScope _scope;




        public CreateGroupModel()
        {

        }
        public void Resolve(ILifetimeScope scope)
        {
            _scope = scope;
            _groupServices = _scope.Resolve<IGroupServices>();
            _httpContextAccessor = _scope.Resolve<IHttpContextAccessor>();
        }
        public CreateGroupModel(IGroupServices groupServices, IHttpContextAccessor httpContextAccessor)
        {
            _groupServices = groupServices;
            _httpContextAccessor = httpContextAccessor;
        }

        internal void CreateGroup()
        {
            var id = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var group = new Group()
            {
                Id = Id,
                Name = Name,
                ApplicationUserId = id


            };
            _groupServices.CreateGroup(group , id);
        }

        internal void DeleteGroup(int id)
        {
            _groupServices.DeleteGroup(id);
        }
    }
}
