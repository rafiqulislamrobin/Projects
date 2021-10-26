using Autofac;
using DataImporter.Areas.User.Models;
using DataImporter.Models.AccountModel;
using DataImporter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataImporter
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<ConfirmFile>().AsSelf();
            builder.RegisterType<CreateGroupModel>().AsSelf();
            builder.RegisterType<EditGroupModel>().AsSelf();
            builder.RegisterType<EmailSenderModel>().AsSelf();
            builder.RegisterType<ExportFileModel>().AsSelf();
            builder.RegisterType<ExportHistoryModel>().AsSelf();
            builder.RegisterType<ExportStatusModel>().AsSelf();
            builder.RegisterType<FilePathModel>().AsSelf();
            builder.RegisterType<ImportHistoryModel>().AsSelf();
            builder.RegisterType<IndexModel>().AsSelf();
            builder.RegisterType<ViewGroupModel>().AsSelf();

            builder.RegisterType<LoginModel>().AsSelf();
            builder.RegisterType<RegisterModel>().AsSelf();
            builder.RegisterType<EmailService>().As<IEmailService>()
             .InstancePerLifetimeScope();
            base.Load(builder);
        }
    }
}
