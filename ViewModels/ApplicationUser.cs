using System;
using Microsoft.AspNetCore.Identity;

namespace ScarletAuth.ViewModels
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            // var guid = Guid.NewGuid().ToString();
            // base.Id = guid;
        }
    }
}