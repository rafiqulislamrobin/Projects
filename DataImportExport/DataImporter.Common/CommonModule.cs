using Autofac;
using DataImporter.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Common
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {


            builder.RegisterType<DatetimeUtility>().As<IDatetimeUtility>()
                .InstancePerLifetimeScope();


            base.Load(builder);
        }
    }
}
