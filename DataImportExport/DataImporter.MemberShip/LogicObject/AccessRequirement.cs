using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.MemberShip.LogicObject
{

    public class AccessRequirement : IAuthorizationRequirement
    {
        public AccessRequirement()
        {
        }
    }
}
