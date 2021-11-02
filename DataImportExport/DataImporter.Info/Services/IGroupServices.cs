using DataImporter.Info.Business_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataImporter.Info.Services
{
   public interface IGroupServices
    {
        void CreateGroup(Group group, Guid id);
        Group LoadGroup(int id);
        void DeleteGroup(int id);
        void UpdateGroup(Group group, Guid id);

        List<Group> LoadAllGroups(Guid id);
        List<Group> LoadGroupsWithContact(Guid id);
        (IList<Group> records, int total, int totalDisplay) GetGroupsList(int pageIndex, int pageSize,
                                                 string searchText, Guid id, string sortText);
    }
}
