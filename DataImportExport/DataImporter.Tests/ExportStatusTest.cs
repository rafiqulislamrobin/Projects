using Autofac.Extras.Moq;
using DataImporter.Info.Business_Object;
using DataImporter.Info.Repositories;
using DataImporter.Info.Services;
using DataImporter.Info.UnitOfWorks;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Diagnostics.CodeAnalysis;
using EO = DataImporter.Info.Entities;

namespace DataImporter.Tests
{

    [ExcludeFromCodeCoverage]
    public class ExportStatusTest
    {
        private AutoMock _mock;
        private Mock<IDataUnitOfWork> _dataUnitOfWorkMock;
        private Mock<IExportStatusRepository> _exportStatusRepositoryMock;
        private ExportServices _exportServices;


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
            _exportServices = _mock.Create<ExportServices>();
            _exportStatusRepositoryMock = _mock.Mock<IExportStatusRepository>();
        }
        [TearDown]
        public void TestCleanup()
        {
            _dataUnitOfWorkMock.Reset();
            _exportStatusRepositoryMock.Reset();

        }
        [Test]
        public void SaveExportHistory_ExportStatusNotExist_throwException()
        {
            ExportStatus exportStatus = null;


            //act //assert
            Should.Throw<InvalidOperationException>(
             () => _exportServices.SaveExportHistory(exportStatus));
        }
        [Test]
        public void SaveExportHistory_ExportStatusExist_SaveToDatabase()
        {
            ExportStatus exportStatus = new()
            {
                Id = 2,
                Email = "abc@mail.com",
                DateTime = DateTime.Now,
                GroupId =2
            };
            EO.ExportStatus exportStatusEntity = new()
            {
                Id = exportStatus.Id,
                Email = exportStatus.Email,
                DateTime = exportStatus.DateTime,
                GroupId =exportStatus.GroupId
            };

            _dataUnitOfWorkMock.Setup(x => x.ExportStatus).Returns(_exportStatusRepositoryMock.Object);
            _exportStatusRepositoryMock.Object.Add(exportStatusEntity);
            //_exportStatusRepositoryMock.Setup(x => x.Add(exportStatusEntity));
            _dataUnitOfWorkMock.Setup(x => x.Save()).Verifiable();

            //act 
            _exportServices.SaveExportHistory(exportStatus);

            //assert
            this.ShouldSatisfyAllConditions(
                () => _dataUnitOfWorkMock.VerifyAll(),
                () => _exportStatusRepositoryMock.VerifyAll()
                );

        }
    }

}
