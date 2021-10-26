using Microsoft.AspNetCore.Identity;
using System;

namespace DataImporter.MemberShip.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public int Age { get; set; }
    }
}
