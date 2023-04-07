using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Domain.Entities
{
    public class Role : IdentityRole<Guid>, IEntity<Guid>
    {
    }
}
