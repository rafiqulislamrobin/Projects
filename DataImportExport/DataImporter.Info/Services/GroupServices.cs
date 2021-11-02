using DataImporter.Info.Business_Object;
using DataImporter.Info.Exceptions;
using DataImporter.Info.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Services
{
    public class GroupServices : IGroupServices
    {
        private readonly IDataUnitOfWork _dataUnitOfWork;
        public GroupServices(IDataUnitOfWork dataUnitOfWork)
        {
            _dataUnitOfWork = dataUnitOfWork;

        }

        public void CreateGroup(Group group, Guid id)
        {
            if (group == null)
                throw new InvalidParameterException("Group was not found");

            if (IsNameAlreadyUsed(group.Name, id))
            {
                throw new DuplicateException("Group name is already used");

            }

            _dataUnitOfWork.Group.Add(
                    new Entities.Group
                    {
                        Id = group.Id,
                        Name = group.Name,
                        ApplicationUserId = group.ApplicationUserId
                    });
            _dataUnitOfWork.Save();

        }
        public void DeleteGroup(int id)
        {
            _dataUnitOfWork.Group.Remove(id);
            _dataUnitOfWork.Save();
        }

        public List<Group> LoadAllGroups(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            var groupEntities = _dataUnitOfWork.Group.GetAll().Where(g => g.ApplicationUserId == id);

            var result = (from g in groupEntities
                          select new Group
                          {
                              Id = g.Id,
                              Name = g.Name
                          }).ToList();
            return result;

        }

        public Group LoadGroup(int id)
        {
            var group = _dataUnitOfWork.Group.GetById(id);
            if (group == null)
            {
                return null;
            }
            return new Group
            {
                Id = group.Id,
                Name = group.Name,
            };
        }
        public void UpdateGroup(Group group, Guid id)
        {
            if (group == null && id == Guid.Empty)
            {
                throw new InvalidParameterException("Group is missing");
             
            }
            if (IsNameAlreadyUsed(group.Name, id))
            {
                throw new DuplicateException("Group name is already used");
            }
            var groupEntity = _dataUnitOfWork.Group.GetById(group.Id);
            if (groupEntity != null)
            {
                groupEntity.Id = group.Id;
                groupEntity.Name = group.Name;
                _dataUnitOfWork.Save();
            }
            else
                throw new InvalidOperationException("Group is not available");

        }


        public (IList<Group> records, int total, int totalDisplay) GetGroupsList(int pageIndex, int pageSize,
                                                        string searchText, Guid id, string sortText)
        {
            var groupData = _dataUnitOfWork.Group.GetDynamic(
               string.IsNullOrWhiteSpace(searchText) ? x => x.ApplicationUserId == id : x => x.Name.Contains
               (searchText) && x.ApplicationUserId == id, sortText, "ApplicationUser", pageIndex, pageSize);


            var resultData = (from groups in groupData.data
                              select new Group
                              {
                                  ApplicationUserId = groups.ApplicationUserId,
                                  Id = groups.Id,
                                  Name = groups.Name

                              }).ToList();

            return (resultData, groupData.total, groupData.totalDisplay);
        }
        private bool IsNameAlreadyUsed(string name, Guid id) =>
          _dataUnitOfWork.Group.GetCount(g => g.Name == name && g.ApplicationUserId == id) > 0;

        public List<Group> LoadGroupsWithContact(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            var groupEntities = _dataUnitOfWork.Group.Get(g => g.ApplicationUserId == id && g.Contacts.Count > 0, "Contacts");

            var result = (from g in groupEntities
                          
                          select new Group
                          {
                              Id = g.Id,
                              Name = g.Name
                          }).ToList();
            return result;
        }
    }
}
