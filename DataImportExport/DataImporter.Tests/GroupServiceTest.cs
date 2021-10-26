using Autofac.Extras.Moq;
using EO = DataImporter.Info.Entities;
using DataImporter.Info.Business_Object;
using DataImporter.Info.Exceptions;
using DataImporter.Info.Repositories;
using DataImporter.Info.Services;
using DataImporter.Info.UnitOfWorks;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace DataImporter.Tests
{
    [ExcludeFromCodeCoverage]
    public class GroupServiceTest
    {
        private AutoMock _mock;
        private Mock<IDataUnitOfWork> _dataUnitOfWorkMock;
        private Mock<IGroupRepository> _groupRepositoryMock;
        private GroupServices _groupservice;
          

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [OneTimeTearDown]
        public void ClassCleanUp()
        {
            _mock?.Dispose();
        }


        [SetUp]
        public void Setup()
        {
            _dataUnitOfWorkMock = _mock.Mock<IDataUnitOfWork>();
            _groupservice = _mock.Create<GroupServices>();
            _groupRepositoryMock = _mock.Mock<IGroupRepository>();
        }
        [TearDown]
        public void TestCleanup()
        {
            _dataUnitOfWorkMock.Reset();
            _groupRepositoryMock.Reset();

        }


        [Test]
        public void UpdateGroup_GroupNotExist_throwException()
        {
           Group group = null;
           Guid id = Guid.Empty;

            //act //assert
            Should.Throw<InvalidParameterException>(
             () => _groupservice.UpdateGroup(group, id));
        }
        [Test]
        //fail
        public void UpdateGroup_IsGroupNameAlreadyExist_throwException()
        {
            Guid id =  Guid.NewGuid();
            Group group = new Group { Id = 2, Name = "asp.net", ApplicationUserId = id };

            var groupEntity = new EO.Group { Id = 2, ApplicationUserId = id, Name = "asp.net" };
           
            _dataUnitOfWorkMock.Setup(x => x.Group).Returns(_groupRepositoryMock.Object);

            _groupRepositoryMock.Object.Add(groupEntity);

            var zzz=  _groupRepositoryMock.Object.GetCount(x => x.Name == groupEntity.Name && x.ApplicationUserId != 
         groupEntity.ApplicationUserId);
          

            //act //assert
            Should.Throw<InvalidOperationException>(
             () => _groupservice.UpdateGroup(group, id));
        }
        [Test]
        public void UpdateGroup_GroupEntityNull_throwException()
        {
            //Arrange
            var group = new Group { Id = 2, Name = "asp.net" };
            var id = Guid.NewGuid();

            EO.Group groupEntity = null; 

            _dataUnitOfWorkMock.Setup(x => x.Group).Returns(_groupRepositoryMock.Object);

            _groupRepositoryMock.Setup(x => x.GetById(group.Id)).Returns(groupEntity);


            //Act and Assert
            Should.Throw<InvalidOperationException>(
                () => _groupservice.UpdateGroup(group, id));
        }
        [Test]
        public void UpdateGroup_GroupEntityExist_SaveGroupInfo()
        {
            //Arrange
            var id = Guid.NewGuid();
            var group = new Group { Id = 2, Name = "asp.net", ApplicationUserId =id };
            

            var groupEntity = new EO.Group { Id = 2, ApplicationUserId = id , Name =group.Name};

            _dataUnitOfWorkMock.Setup(x => x.Group).Returns(_groupRepositoryMock.Object);
            _groupRepositoryMock.Setup(x => x.GetById(group.Id)).Returns(groupEntity);
            _dataUnitOfWorkMock.Setup(x => x.Save()).Verifiable();


            //Act 
            _groupservice.UpdateGroup(group, id);

            //Assert
             this.ShouldSatisfyAllConditions(
                () => groupEntity.ApplicationUserId.ShouldBe(group.ApplicationUserId),
                () =>groupEntity.Id.ShouldBe(group.Id),
                () => groupEntity.Name.ShouldBe(group.Name),
                () => _dataUnitOfWorkMock.Verify(),
                () => _groupRepositoryMock.Verify()
                );
          
        }
        [Test]
        public void LoadGroup_GroupNotExist_throwException()
        {
            //Arrange
            var id = 2;
            Group group = null; //should
            EO.Group groupEntity = null;

            _dataUnitOfWorkMock.Setup(x => x.Group).Returns(_groupRepositoryMock.Object);
            _groupRepositoryMock.Setup(x => x.GetById(id)).Returns(groupEntity);


            //act //assert
            Assert.IsNull(_groupservice.LoadGroup(id));
            //using should
            Equals(group, _groupservice.LoadGroup(id));
        }
        [Test]
        public void LoadGroup_GroupExist_ReturnGroup()
        {
            //Arrange
            var id = 2;
            Group group = new() { Id = id, Name = "asp.net", ApplicationUserId=Guid.NewGuid() };
          

            _dataUnitOfWorkMock.Setup(x => x.Group).Returns(_groupRepositoryMock.Object);
            _groupRepositoryMock.Setup(x => x.GetById(id)).Returns(
                new EO.Group { Id=group.Id, Name = group.Name });


            //act //assert
            Should.Equals(_groupservice.LoadGroup(id), group);
            
        }
        [Test]
        public void LoadAllGroups_UserNotExist_ReturnNull()
        {
            //Arrange
            Guid id = Guid.Empty;

            //act //assert
            Assert.IsNull(_groupservice.LoadAllGroups(id));
      
        }
        [Test]
        public void DeleteGroup_Save()
        {
            //Arrange
            var id = 1;
            var groupEntity = new EO.Group { Id = id, ApplicationUserId = Guid.NewGuid(), Name = "C#" };

            _dataUnitOfWorkMock.Setup(x => x.Group).Returns(_groupRepositoryMock.Object);

            _groupRepositoryMock.Object.Add(groupEntity);
            //act //assert
            _groupservice.DeleteGroup(id);

        }
        [Test]
        public void CreateGroup_Save()
        {
            //Arrange
            var id = Guid.NewGuid();
            var group = new Group { Id = 2, Name = "asp.net", ApplicationUserId = id };


            var groupEntity = new EO.Group { Id = 2, ApplicationUserId = id, Name = group.Name };

            _dataUnitOfWorkMock.Setup(x => x.Group).Returns(_groupRepositoryMock.Object);
            _groupRepositoryMock.Object.Add(groupEntity);
            _dataUnitOfWorkMock.Setup(x => x.Save()).Verifiable();


            //Act 
            _groupservice.CreateGroup(group, id);

            //Assert
            this.ShouldSatisfyAllConditions(
               () => groupEntity.ApplicationUserId.ShouldBe(group.ApplicationUserId),
               () => groupEntity.Name.ShouldBe(group.Name),
               () => _dataUnitOfWorkMock.Verify(),
               () => _groupRepositoryMock.Verify()
               );
        }
        [Test]
        public void CreateGroup_GroupNotExist_throwException()
        {
            Group group = null;
            Guid id = Guid.Empty;

            //act //assert
            Should.Throw<InvalidParameterException>(
             () => _groupservice.CreateGroup(group, id));
        }


    }
}